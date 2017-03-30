// The MIT License
// 
// Copyright (c) 2017 TruRating Ltd. https://www.trurating.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System.Diagnostics;
using System.Threading;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Device;
using TruRating.TruModule.V2xx.Messages;
using TruRating.TruModule.V2xx.Network;
using TruRating.TruModule.V2xx.Settings;
using TruRating.TruModule.V2xx.Util;

namespace TruRating.TruModule.V2xx
{
    public abstract class TruModule
    {
        private readonly AutoResetEvent _dwellTimeExtendAutoResetEvent = new AutoResetEvent(false);
        private readonly ILogger _logger;
        private readonly ITruServiceClient _truServiceClient;
        protected readonly IDevice Device;
        protected readonly IReceiptManager ReceiptManager;
        protected readonly ISettings Settings;
        protected readonly ITruServiceMessageFactory TruServiceMessageFactory;
        private int _dwellTimeExtendMs;
        private volatile bool _isCancelled;
        private volatile bool _isQuestionRunning;

        protected TruModule(IDevice device, IReceiptManager receiptManager, ITruServiceClient truServiceClient, ILogger logger,
            ITruServiceMessageFactory truServiceMessageFactory, ISettings settings)
        {
            Device = device;
            _truServiceClient = truServiceClient;
            _logger = logger;
            TruServiceMessageFactory = truServiceMessageFactory;
            Settings = settings;
            ReceiptManager = receiptManager;
            SessionId = DateTimeProvider.UtcNow.Ticks.ToString();
            IsActivated(true);
        }

        protected string SessionId { get; set; }
    
        public bool IsActivated(bool force)
        {
            if (Settings.ActivationRecheck > DateTimeProvider.UtcNow && !force)
            {
                _logger.Debug("Not querying TruService status, next check at {0}. IsActive is {1}",
                    Settings.ActivationRecheck, Settings.IsActivated);
                return Settings.IsActivated;
            }
            var status =
                _truServiceClient.Send(TruServiceMessageFactory.AssemblyRequestQuery(Device,ReceiptManager, Settings.PartnerId,
                    Settings.MerchantId, Settings.TerminalId, SessionId, force));

            var responseStatus = status != null ? status.Item as ResponseStatus : null;
            if (responseStatus != null)
            {
                Settings.ActivationRecheck = DateTimeProvider.UtcNow.AddSeconds(responseStatus.TimeToLive);
                Settings.IsActivated = responseStatus.IsActive;
            }
            return Settings.IsActivated;
        }

        public void CancelRating()
        {
            _isCancelled=true; //Set flag to show question has been cancelled
            if (_isQuestionRunning)
            {
                if (Settings.Trigger == Trigger.DWELLTIMEEXTEND)
                {
                    _logger.Debug("Waiting {0} to cancel rating", _dwellTimeExtendMs);
                    _dwellTimeExtendAutoResetEvent.WaitOne(_dwellTimeExtendMs); //Wait for dwelltime extend to finish
                    if (_isQuestionRunning) //recheck _isQuestionRunning because customer may have provided rating
                    {
                        Device.ResetDisplay(); //Force the 1AQ1KR loop to exit and release control of the PED
                    }
                }
                else
                {
                    Device.ResetDisplay(); //Force the 1AQ1KR loop to exit and release control of the PED
                }
                _logger.Debug("Cancelled rating");
            }
        }

        protected Response SendRequest(Request request)
        {
            return _truServiceClient.Send(request);
        }

        protected void DoRating(Request request)
        {
            _isCancelled=false;
            if (!(request.Item is RequestQuestion))
            {
                _logger.Info("Request was not a question");
                return;
            }
            Settings.Trigger = ((RequestQuestion) request.Item).Trigger;
            var response = _truServiceClient.Send(request);
            if (response == null)
            {
                _logger.Info("Response was null");
                return;
            }
            ResponseQuestion question;
            ResponseReceipt[] receipts;
            ResponseScreen[] screens;
            var rating = new RequestRating //Have a question, construct the basic rating record
            {
                DateTime = DateTimeProvider.UtcNow,
                Rfc1766 = Device.GetCurrentLanguage(),
                Value = -4 //initial state is "cannot show question"
            };
            //Look through the response for a valid question
            ResponseScreen responseScreen = null;
            if (QuestionAvailable(response, Device.GetCurrentLanguage(), out question, out receipts, out screens))
            {
                var sw = new Stopwatch();
                sw.Start();
                var trigger = ((RequestQuestion) request.Item).Trigger; //Grab the trigger from the question request
                var timeoutMs = question.TimeoutMs;
                if (trigger == Trigger.DWELLTIMEEXTEND)
                {
                    timeoutMs = int.MaxValue;
                    _dwellTimeExtendMs = question.TimeoutMs;
                }
                _isQuestionRunning=true;
                rating.Value = Device.Display1AQ1KR(question.Value, timeoutMs);
                //Wait for the user input for the specified period
                _isQuestionRunning=false;
                _dwellTimeExtendAutoResetEvent.Set();
                    //Signal to CancelRating that question has been answered when called in another thread.
                sw.Stop();
                rating.ResponseTimeMs = (int) sw.ElapsedMilliseconds; //Set the response time
                var responseReceipt = GetResponseReceipt(receipts, rating.Value < 0 ? When.NOTRATED : When.RATED);
                responseScreen = GetResponseScreen(screens, rating.Value < 0 ? When.NOTRATED : When.RATED);
                ReceiptManager.AppendReceipt(responseReceipt);
            }
            _truServiceClient.Send(TruServiceMessageFactory.AssembleRequestRating(request, rating));
            if ( responseScreen != null && (!_isCancelled || responseScreen.Priority))
            {
                Device.DisplayMessage(responseScreen.Value, responseScreen.TimeoutMs);
            }
        }



        internal static bool QuestionAvailable(Response response, string language, out ResponseQuestion responseQuestion,
            out ResponseReceipt[] responseReceipts, out ResponseScreen[] responseScreens)
        {
            responseQuestion = null;
            responseReceipts = null;
            responseScreens = null;
            var item = response.Item as ResponseDisplay;
            if (item != null)
            {
                var responseDisplay = item;
                foreach (var responseLanguage in responseDisplay.Language)
                {
                    if (responseLanguage.Rfc1766 == language && responseLanguage.Question!=null)
                    {
                        responseQuestion = responseLanguage.Question;
                        responseReceipts = responseLanguage.Receipt;
                        responseScreens = responseLanguage.Screen;
                        return true;
                    }
                }
                return false;
            }

            return false;
        }

        internal static string GetResponseReceipt(ResponseReceipt[] responseReceipts, When when)
        {
            if (responseReceipts == null)
                return null;
            foreach (var responseReceipt in responseReceipts)
            {
                if (responseReceipt.When == when)
                {
                    return responseReceipt.Value;
                }
            }
            return null;
        }

        internal static ResponseScreen GetResponseScreen(ResponseScreen[] responseScreens, When whenToDisplay)
        {
            if (responseScreens == null)
                return null;
            foreach (var responseScreen in responseScreens)
            {
                if (responseScreen.When == whenToDisplay) //If this response element matches the state of the screen.
                {
                    return responseScreen;
                }
            }
            return null;
        }
    }
}
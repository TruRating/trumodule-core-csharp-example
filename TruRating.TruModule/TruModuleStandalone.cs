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
using System;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.Device;
using TruRating.TruModule.Messages;
using TruRating.TruModule.Network;
using TruRating.TruModule.Settings;
using TruRating.TruModule.Util;

namespace TruRating.TruModule
{
    public class TruModuleStandalone : TruModule, ITruModuleStandalone
    {
        public TruModuleStandalone(IDevice device, IReceiptManager receiptManager, ITruServiceClient truServiceClient, ILogger logger,
            ITruServiceMessageFactory truServiceMessageFactory, ISettings settings)
            : base(device, receiptManager, truServiceClient, logger, truServiceMessageFactory, settings)
        {
        }

        public void DoRating()
        {
            try
            {
                SessionId = DateTimeProvider.UtcNow.Ticks.ToString();
                if (IsActivated(bypassTruServiceCache: false))
                {
                    var request = TruServiceMessageFactory.AssembleRequestQuestion(new RequestParams(Settings, SessionId), Device, ReceiptManager, Settings.Trigger);
                    DoRating(request);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error in DoRating");
            }
        }

        public void SendTransaction(RequestTransaction requestTransaction)
        {
            try
            {
                if (IsActivated(bypassTruServiceCache: false))
                {
                    var request = TruServiceMessageFactory.AssembleRequestTransaction(new RequestParams(Settings, SessionId), requestTransaction);
                    SendRequest(request);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error in SendTransaction");
            }

        }

        public LookupOption[] GetLookups(LookupName lookupName)
        {
            try
            {
                var request = TruServiceMessageFactory.AssembleRequestLookup(new RequestParams(Settings, SessionId), Device, ReceiptManager, lookupName);

                var responseLookup = SendRequest(request);

                if (responseLookup == null)
                    return null;

                var responseStatus = responseLookup.Item as ResponseLookup;
                if (responseStatus != null)
                {
                    foreach (var language in responseStatus.Language)
                    {
                        if (language.Rfc1766 == Device.GetCurrentLanguage())
                        {
                            return language.Option;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error in GetLookups");
            }
            return null;
        }

        public bool Activate(int sectorNode, string timeZone, PaymentInstant paymentInstant, string emailAddress, string password, string address, string mobileNumber, string merchantName, string businessName)
        {
            try
            {
                var status =
                    SendRequest(TruServiceMessageFactory.AssembleRequestActivate(
                        new RequestParams(Settings, SessionId), Device, ReceiptManager, sectorNode, timeZone,
                        PaymentInstant.PAYBEFORE,
                        emailAddress, password, address, mobileNumber, merchantName, businessName));
                var responseStatus = status.Item as ResponseStatus;
                if (responseStatus != null)
                {
                    Settings.ActivationRecheck = DateTimeProvider.UtcNow.AddMinutes(responseStatus.TimeToLive);
                    Settings.IsActivated = responseStatus.IsActive;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error in Activate with reg form");
            }
            return Settings.IsActivated;

        }
        public bool Activate(string registrationCode)
        {
            try
            {
                var status = SendRequest(TruServiceMessageFactory.AssembleRequestActivate(new RequestParams(Settings, SessionId), Device, ReceiptManager, registrationCode));
                var responseStatus = status.Item as ResponseStatus;
                if (responseStatus != null)
                {
                    Settings.ActivationRecheck = DateTimeProvider.UtcNow.AddMinutes(responseStatus.TimeToLive);
                    Settings.IsActivated = responseStatus.IsActive;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error in Activate with reg code");
            }
            return Settings.IsActivated;
        }


    }
}
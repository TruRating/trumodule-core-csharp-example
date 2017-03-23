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
using System.Collections.Generic;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Device;
using TruRating.TruModule.V2xx.Messages;
using TruRating.TruModule.V2xx.Network;
using TruRating.TruModule.V2xx.Settings;
using TruRating.TruModule.V2xx.Util;

namespace TruRating.TruModule.V2xx
{
    public class TruModuleStandalone : TruModule, ITruModuleStandalone
    {
        public TruModuleStandalone(IDevice device, IReceiptManager receiptManager, ITruServiceClient truServiceClient, ILogger logger,
            ITruServiceMessageFactory truServiceMessageFactory, ISettings settings)
            : base(device,receiptManager, truServiceClient, logger, truServiceMessageFactory, settings)
        {
        }

        public void DoRating()
        {
            SessionId = DateTimeProvider.UtcNow.Ticks.ToString();
            if (IsActivated(false))
            {
                var request = TruServiceMessageFactory.AssembleRequestQuestion(Device,ReceiptManager, Settings.PartnerId, Settings.MerchantId, Settings.TerminalId, SessionId, Trigger.PAYMENTREQUEST);
                DoRating(request);
            }
        }

        public void SendTransaction(RequestTransaction requestTransaction)
        {
            if (IsActivated(false))
            {
                var request = TruServiceMessageFactory.AssembleRequestTransaction(Settings.PartnerId, SessionId,
                    Settings.MerchantId, Settings.TerminalId, requestTransaction);
                SendRequest(request);
            }
        }

        public Dictionary<int, string> GetLookups(LookupName lookupName)
        {
            var result = new List<KeyValuePair<int, string>>();

            var request = TruServiceMessageFactory.AssembleRequestLookup(Device, ReceiptManager, Settings.PartnerId, Settings.MerchantId,
                Settings.TerminalId, SessionId, lookupName);

            var responseLookup = SendRequest(request);

            var responseStatus = responseLookup.Item as ResponseLookup;
            if (responseStatus != null)
            {
                var optionNumber = 0;
                foreach (var language in responseStatus.Language)
                {
                    if (language.Rfc1766 == Device.GetCurrentLanguage())
                    {
                        if (language.Option != null)
                        {
                            foreach (var lookupOption in language.Option)
                            {
                                result.AddRange(PrintLookups(lookupOption, 1, optionNumber));
                            }
                        }
                    }
                }
            }

            return ExtensionMethods.ToDictionary(result, x => x.Key, x => x.Value);
        }

        private IEnumerable<KeyValuePair<int, string>> PrintLookups(LookupOption lookupOption, int depth,
            int optionNumber)
        {
            var result = new List<KeyValuePair<int, string>>();

            if (lookupOption.Value != null)
            {
                optionNumber++;
                result.Add(new KeyValuePair<int, string>(optionNumber, lookupOption.Value));
            }
            Device.DisplayMessage((lookupOption.Value == null ? "N/A" : "\"" + optionNumber + "\"") +
                                  "".PadLeft(depth, ' ') + lookupOption.Text + " (" + lookupOption.Value + ")");
            if (lookupOption.Option != null)
            {
                foreach (var option in lookupOption.Option)
                {
                    result.AddRange(PrintLookups(option, depth + 1, optionNumber));
                }
            }
            return result;
        }

        public bool Activate(int sectorNode, string timeZone, PaymentInstant paymentInstant, string emailAddress, string password, string address, string mobileNumber, string merchantName, string businessName)
        {
            var status =SendRequest(TruServiceMessageFactory.AssembleRequestActivate(Device, ReceiptManager, Settings.PartnerId, SessionId,
                        Settings.MerchantId, Settings.TerminalId, sectorNode, timeZone, PaymentInstant.PAYBEFORE,
                        emailAddress, password, address, mobileNumber, merchantName, businessName));
            var responseStatus = status.Item as ResponseStatus;
            if (responseStatus != null)
            {
                Settings.ActivationRecheck = DateTime.UtcNow.AddMinutes(responseStatus.TimeToLive);
                Settings.IsActivated = responseStatus.IsActive;
            }
            return Settings.IsActivated;
        }
        public bool Activate(string registrationCode)
        {
            var status = SendRequest(TruServiceMessageFactory.AssembleRequestActivate(Device, ReceiptManager, Settings.PartnerId, SessionId,
                       Settings.MerchantId, Settings.TerminalId, registrationCode));
            var responseStatus = status.Item as ResponseStatus;
            if (responseStatus != null)
            {
                Settings.ActivationRecheck = DateTime.UtcNow.AddMinutes(responseStatus.TimeToLive);
                Settings.IsActivated = responseStatus.IsActive;
            }
            return Settings.IsActivated;
        }
    }
}
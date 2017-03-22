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
using TruRating.TruModule.V2xx.Helpers;
using TruRating.TruModule.V2xx.Network;
using TruRating.TruModule.V2xx.Serialization;
using TruRating.TruModule.V2xx.Settings;

namespace TruRating.TruModule.V2xx.Module
{
    public class TruModuleStandalone : TruModule, ITruModuleStandalone
    {
        public TruModuleStandalone(IPinPad pinPad, IPrinter printer, ITruServiceClient<Request, Response> truServiceClient, ILogger logger,
            ITruServiceMessageFactory truServiceMessageFactory, ISettings settings)
            : base(pinPad,printer, truServiceClient, logger, truServiceMessageFactory, settings)
        {
        }

        public void DoRating()
        {
            SessionId = DateTimeProvider.UtcNow.Ticks.ToString();
            if (IsActivated(false))
            {
                var request = TruServiceMessageFactory.AssembleRequestQuestion(PinPad,Printer, Settings.PartnerId,
                    Settings.MerchantId, Settings.TerminalId, SessionId, Trigger.PAYMENTREQUEST);
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

        private Dictionary<int, string> GetLookupResponse(LookupName lookupName)
        {
            var result = new List<KeyValuePair<int, string>>();

            var request = TruServiceMessageFactory.AssembleRequestLookup(PinPad, Printer, Settings.PartnerId, Settings.MerchantId,
                Settings.TerminalId, SessionId, lookupName);

            var responseLookup = SendRequest(request);

            var responseStatus = responseLookup.Item as ResponseLookup;
            if (responseStatus != null)
            {
                var optionNumber = 0;
                foreach (var language in responseStatus.Language)
                {
                    if (language.Rfc1766 == PinPad.GetCurrentLanguage())
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
            PinPad.DisplayMessage((lookupOption.Value == null ? "N/A" : "\"" + optionNumber + "\"") +
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

        private string Lookup(LookupName lookupName)
        {
            var options = GetLookupResponse(lookupName);
            while (true)
            {
                var selectedOption = PinPad.ReadLine("Please pick a numbered " + lookupName);
                int selectedOptionNumber;
                if (int.TryParse(selectedOption, out selectedOptionNumber))
                {
                    if (options.ContainsKey(selectedOptionNumber))
                    {
                        return options[selectedOptionNumber];
                    }
                    PinPad.DisplayMessage("Invalid option");
                }
            }
        }

        public void Activate()
        {
            if (Settings.IsActivated)
                return;
            PinPad.DisplayMessage("This device is not registered!");
            var registrationCode =
                PinPad.ReadLine(
                    "Type your registration code, Press ENTER to register via form input or type SKIP to skip registration");
            Response status;
            if (string.IsNullOrEmpty(registrationCode))
            {
                var sectorNode = int.Parse(Lookup(LookupName.SECTORNODE));
                var timeZone = Lookup(LookupName.TIMEZONE);
                string emailAddress = null;
                string password = null;
                string address = null;
                string mobileNumber = null;
                string businessName = null;
                string merchantName = null;

                while (string.IsNullOrEmpty(emailAddress))
                    emailAddress = PinPad.ReadLine("Enter your email address (required)");
                while (string.IsNullOrEmpty(password))
                    password = PinPad.ReadLine("Enter a password (required)");
                while (string.IsNullOrEmpty(address))
                    address = PinPad.ReadLine("Enter your postal address");
                while (string.IsNullOrEmpty(mobileNumber))
                    mobileNumber = PinPad.ReadLine("Enter your mobile number, e.g. +44 (1234) 787123");
                while (string.IsNullOrEmpty(businessName))
                    businessName = PinPad.ReadLine("Enter your business name, e.g. McDonalds");
                while (string.IsNullOrEmpty(merchantName))
                    merchantName = PinPad.ReadLine("Enter your outlet name, e.g. McDonalds Fleet Street");
                status =
                    SendRequest(TruServiceMessageFactory.AssembleRequestActivate(PinPad,Printer, Settings.PartnerId, SessionId,
                        Settings.MerchantId, Settings.TerminalId, sectorNode, timeZone, PaymentInstant.PAYBEFORE,
                        emailAddress, password, address, mobileNumber, merchantName, businessName));
            }
            else if (registrationCode.Equals("SKIP", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            else
            {
                status =
                    SendRequest(TruServiceMessageFactory.AssembleRequestActivate(PinPad,Printer, Settings.PartnerId, SessionId,
                        Settings.MerchantId, Settings.TerminalId, registrationCode));
            }

            var responseStatus = status.Item as ResponseStatus;
            if (responseStatus != null)
            {
                Settings.ActivationRecheck = DateTime.UtcNow.AddMinutes(responseStatus.TimeToLive);
                Settings.IsActivated = responseStatus.IsActive;
                if (Settings.IsActivated)
                {
                    PinPad.DisplayMessage("This device is activated");
                }
                else
                {
                    PinPad.DisplayMessage("This device is not activated");
                }
            }
        }

        public bool IsActivated()
        {
            return Settings.IsActivated;
        }
    }
}
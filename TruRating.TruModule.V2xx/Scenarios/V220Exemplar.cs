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
using System.ComponentModel.Design;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Environment;
using TruRating.TruModule.V2xx.Messages;

namespace TruRating.TruModule.V2xx.Scenarios
{
    public class V220Exemplar : ExemplarBase
    {
        private readonly ILogger _logger;
        private readonly IDevice _device;
        private readonly ITsiV220Messages _tsiV220Messages;
        private readonly V200Exemplar _v200Exemplar;
        private readonly V210PosEventListExemplar _v210PosEventListExemplar;
        private readonly V210PosEventsExemplar _v210PosEventsExemplar;
        private bool _forceQuery = true;

        public V220Exemplar(ILogger logger, ISettings settings, IDevice device, ITsiV220Messages tsiV220Messages,
            V200Exemplar v200Exemplar, V210PosEventsExemplar v210PosEventsExemplar,
            V210PosEventListExemplar v210PosEventListExemplar) : base(logger, settings, device)
        {
            _device = device;
            _tsiV220Messages = tsiV220Messages;
            _v200Exemplar = v200Exemplar;
            _v210PosEventsExemplar = v210PosEventsExemplar;
            _v210PosEventListExemplar = v210PosEventListExemplar;
            _logger = logger;
        }

        public override void Scenario()
        {
            if (IsActivated())
            {
                switch (Settings.PosIntegration)
                {
                    case PosIntegration.None:
                        _v200Exemplar.Scenario();
                        break;
                    case PosIntegration.Events:
                        _v210PosEventsExemplar.Scenario();
                        break;
                    case PosIntegration.EventList:
                        _v210PosEventListExemplar.Scenario();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                Activate();
            }
        }

        public override bool IsApplicable()
        {
            return Settings.TsiVersion == TsiVersion.V220;
        }

        private string Lookup(LookupName lookupName)
        {
            var options = GetLookupResponse(lookupName);
            while (true)
            {
                var selectedOption = _device.ReadLine("Please pick a numbered " + lookupName);
                int selectedOptionNumber;
                if (int.TryParse(selectedOption, out selectedOptionNumber))
                {
                    if (options.ContainsKey(selectedOptionNumber))
                    {

                        return options[selectedOptionNumber];
                    }
                    _logger.WriteLine(ConsoleColor.Red, "Invalid option");
                }
            }
        }

        private Dictionary<int, string> GetLookupResponse(LookupName lookupName)
        {
            var result = new List<KeyValuePair<int, string>>();
            var responseLookup = _tsiV220Messages.SendRequestLookup(lookupName);
            var responseStatus = responseLookup.Item as ResponseLookup;
            if (responseStatus != null)
            {
                var optionNumber = 0;
                foreach (var language in responseStatus.Language)
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

            return ExtensionMethods.ToDictionary(result, x => x.Key, x => x.Value);
        }

        private IEnumerable<KeyValuePair<int, string>> PrintLookups(LookupOption lookupOption, int depth, int optionNumber)
        {
            var result = new List<KeyValuePair<int, string>>();
            
            if (lookupOption.Value != null)
            {
                optionNumber++;
                result.Add( new KeyValuePair<int, string>(optionNumber, lookupOption.Value));
            }
            _device.DisplayMessage((lookupOption.Value == null ? "N/A" : "\"" + optionNumber + "\"")  + "".PadLeft(depth, ' ') + lookupOption.Text + " (" + lookupOption.Value + ")");
            if (lookupOption.Option != null)
            {
                foreach (var option in lookupOption.Option)
                {
                    result.AddRange(PrintLookups(option, depth + 1, optionNumber));
                }
            }
            return result;
        }

        private void Activate()
        {
            _device.DisplayMessage("This device is not registered!");
            var registrationCode = _device.ReadLine("Type your registration code, or press enter to register with form data");
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
                    emailAddress = _device.ReadLine("Enter your email address (required)");
                while (string.IsNullOrEmpty(password))
                    password = _device.ReadLine("Enter a password (required)");
                while(string.IsNullOrEmpty(address))
                    address = _device.ReadLine("Enter your postal address");
                while (string.IsNullOrEmpty(mobileNumber))
                    mobileNumber = _device.ReadLine("Enter your mobile number, e.g. +44 (1234) 787123");
                while (string.IsNullOrEmpty(businessName))
                    businessName = _device.ReadLine("Enter your business name, e.g. McDonalds");
                while (string.IsNullOrEmpty(merchantName))
                    merchantName = _device.ReadLine("Enter your outlet name, e.g. McDonalds Fleet Street");
                status = _tsiV220Messages.SendRequestActivate(sectorNode, timeZone, PaymentInstant.PAYBEFORE, emailAddress, password,address,mobileNumber, merchantName, businessName);
            }
            else
            {
                status = _tsiV220Messages.SendRequestActivate(registrationCode);
            }

            var responseStatus = status.Item as ResponseStatus;
            if (responseStatus != null)
            {
                Settings.ActivationRecheck = DateTime.UtcNow.AddMinutes(responseStatus.TimeToLive);
                Settings.IsActivated = responseStatus.IsActive;
                if (Settings.IsActivated)
                {
                    _logger.WriteLine(ConsoleColor.Green, "This device is activated");
                }
                else
                {
                    _logger.WriteLine(ConsoleColor.Red, "This device is not activated");
                }
            }
        }

        private bool IsActivated()
        {
            if (Settings.ActivationRecheck > DateTime.UtcNow)
            {
                return Settings.IsActivated;
            }
            var status = _tsiV220Messages.SendRequestQuery(_forceQuery);
            var responseStatus = status.Item as ResponseStatus;
            if (responseStatus != null)
            {
                if (_forceQuery)
                {
                    _forceQuery = false;
                }
                Settings.ActivationRecheck = DateTime.UtcNow.AddSeconds(responseStatus.TimeToLive);
                Settings.IsActivated = responseStatus.IsActive;
            }
            return Settings.IsActivated;
        }
    }
}
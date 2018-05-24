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
using TruRating.TruModule.ConsoleRunner.Device;
using TruRating.TruModule.ConsoleRunner.Environment;
using TruRating.TruModule.ConsoleRunner.Settings;
using TruRating.TruModule.Device;
using TruRating.TruModule.Util;

namespace TruRating.TruModule.ConsoleRunner.UseCase
{
    public class StandaloneUseCase : UseCaseBase
    {
        private readonly ConsoleSettings _consoleSettings;
        private readonly IConsoleLogger _consoleLogger;
        private ITruModuleStandalone _truModule;

        public StandaloneUseCase(IConsoleLogger consoleLogger, ConsoleSettings consoleSettings, IDevice device, IReceiptManager receiptManager)
            : base(consoleLogger, consoleSettings, device,receiptManager)
        {
            _consoleLogger = consoleLogger;
            _consoleSettings = consoleSettings;
        }

        public override void Init()
        {
            _truModule = new TruModuleStandalone(_consoleLogger, _consoleSettings.TruModuleSettings, Device, ReceiptManager);
            if (!_truModule.IsActivated(bypassTruServiceCache:false))
            {
                _consoleLogger.WriteLine(ConsoleColor.Gray,"Standalone UseCase: Not activated at start-up, prompting registration");
                Activate();
            }
        }

        private string ChooseLookupOption(LookupName lookupName)
        {
            var lookups = _truModule.GetLookups(lookupName);

            var options = PrintLookupOptions(lookups);

            while (true)
            {
                var selectedOption = ConsoleLogger.ReadLine("Please pick a numbered " + lookupName);
                int selectedOptionNumber;
                if (int.TryParse(selectedOption, out selectedOptionNumber))
                {
                    if (options.ContainsKey(selectedOptionNumber))
                    {
                        return options[selectedOptionNumber];
                    }
                    Device.DisplayMessage("Invalid option");
                }
            }
        }

        private Dictionary<int, string> PrintLookupOptions(IEnumerable<LookupOption> lookups)
        {
            var result = new List<KeyValuePair<int, string>>();
            var optionNumber = 0;
            foreach (var lookupOption in lookups)
            {
                result.AddRange(RecurseLookupOptions(lookupOption, 1,ref optionNumber));
            }

            var options = ExtensionMethods.ToDictionary(result, x => x.Key, x => x.Value);
            return options;
        }


        private IEnumerable<KeyValuePair<int, string>> RecurseLookupOptions(LookupOption lookupOption, int depth, ref int optionNumber)
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
                    result.AddRange(RecurseLookupOptions(option, depth + 1, ref optionNumber));
                }
            }
            return result;
        }

        private void Activate()
        {
            if (_truModule.IsActivated(bypassTruServiceCache:false))
                return;
            Device.DisplayMessage("This device is not registered!");
            var registrationCode =
                ConsoleLogger.ReadLine(
                    "Type your registration code, Press ENTER to register via form input or type SKIP to skip registration");
            if (string.IsNullOrEmpty(registrationCode))
            {
                var sectorNode = int.Parse(ChooseLookupOption(LookupName.SECTORNODE));
                var timeZone = ChooseLookupOption(LookupName.TIMEZONE);
                string emailAddress = null;
                string password = null;
                string address = null;
                string mobileNumber = null;
                string businessName = null;
                string merchantName = null;

                while (string.IsNullOrEmpty(emailAddress))
                    emailAddress = ConsoleLogger.ReadLine("Enter your email address (required)");
                while (string.IsNullOrEmpty(password))
                    password = ConsoleLogger.ReadLine("Enter a password (required)");
                while (string.IsNullOrEmpty(address))
                    address = ConsoleLogger.ReadLine("Enter your postal address");
                while (string.IsNullOrEmpty(mobileNumber))
                    mobileNumber = ConsoleLogger.ReadLine("Enter your mobile number, e.g. +44 (1234) 787123");
                while (string.IsNullOrEmpty(businessName))
                    businessName = ConsoleLogger.ReadLine("Enter your business name, e.g. Speedy Food");
                while (string.IsNullOrEmpty(merchantName))
                    merchantName = ConsoleLogger.ReadLine("Enter your outlet name, e.g. Speedy Food Fleet Street");
                _truModule.Activate(sectorNode, timeZone, PaymentInstant.PAYBEFORE,
                        emailAddress, password, address, mobileNumber, merchantName, businessName);
            }
            else if (registrationCode.Equals("SKIP", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            else
            {
                _truModule.Activate(registrationCode);
            }
            
            if (_truModule.IsActivated(bypassTruServiceCache:false))
            {
                Device.DisplayMessage("This device is activated");
            }
            else
            {
                Device.DisplayMessage("This device is not activated");
            }
        }
        public override void Example()
        {
            _consoleLogger.WriteLine(ConsoleColor.Gray, "Payment Application: About to make a payment");
            _truModule.DoRating();
            _consoleLogger.WriteLine(ConsoleColor.Gray, "Payment Application: Sending Transaction");
            _truModule.SendTransaction(new RequestTransaction
            {
                Amount = Rand.Next(1000, 2000),
                Currency = 826,
                DateTime = DateTime.UtcNow,
                Gratuity = 0,
                Id = Guid.NewGuid().ToString(),
                Result = TransactionResult.APPROVED
            });
        }

        public override bool IsApplicable()
        {
            return _consoleSettings.PosIntegration == PosIntegration.None;
        }
    }
}

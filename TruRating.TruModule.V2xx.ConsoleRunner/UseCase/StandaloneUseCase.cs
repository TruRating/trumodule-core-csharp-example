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
using TruRating.TruModule.V2xx.ConsoleRunner.Environment;
using TruRating.TruModule.V2xx.Device;
using TruRating.TruModule.V2xx.Module;
using TruRating.TruModule.V2xx.Network;
using TruRating.TruModule.V2xx.Security;
using TruRating.TruModule.V2xx.Serialization;

namespace TruRating.TruModule.V2xx.ConsoleRunner.UseCase
{
    public class StandaloneUseCase : UseCaseBase
    {
        private readonly IConsoleSettings _consoleSettings;
        private readonly IConsoleIo _consoleIo;
        private ITruModuleStandalone _truModule;

        public StandaloneUseCase(IConsoleIo consoleIo, IConsoleSettings consoleSettings, IPinPad pinPad, IPrinter printer)
            : base(consoleIo, consoleSettings, pinPad,printer)
        {
            _consoleIo = consoleIo;
            _consoleSettings = consoleSettings;
        }

        public override void Init()
        {
            var truServiceClient = TruServiceClient<Request, Response>.CreateDefault(_consoleSettings.HttpTimeoutMs,
                _consoleSettings.TruServiceUrl, _consoleIo,
                new MacSignatureCalculator(_consoleSettings.TransportKey, _consoleIo));
            _truModule = new TruModuleStandalone(PinPad,Printer, truServiceClient, _consoleIo, new TruServiceMessageFactory(), _consoleSettings);
            if (!_truModule.IsActivated())
            {
                _consoleIo.WriteLine(ConsoleColor.Gray,"Standalone UseCase: Not activated at start-up, prompting registration");
                Activate();
            }
        }
        private string Lookup(LookupName lookupName)
        {
            var options = _truModule.GetLookups(lookupName);
            while (true)
            {
                var selectedOption = ConsoleIo.ReadLine("Please pick a numbered " + lookupName);
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
        private void Activate()
        {
            if (_truModule.IsActivated())
                return;
            PinPad.DisplayMessage("This device is not registered!");
            var registrationCode =
                ConsoleIo.ReadLine(
                    "Type your registration code, Press ENTER to register via form input or type SKIP to skip registration");
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
                    emailAddress = ConsoleIo.ReadLine("Enter your email address (required)");
                while (string.IsNullOrEmpty(password))
                    password = ConsoleIo.ReadLine("Enter a password (required)");
                while (string.IsNullOrEmpty(address))
                    address = ConsoleIo.ReadLine("Enter your postal address");
                while (string.IsNullOrEmpty(mobileNumber))
                    mobileNumber = ConsoleIo.ReadLine("Enter your mobile number, e.g. +44 (1234) 787123");
                while (string.IsNullOrEmpty(businessName))
                    businessName = ConsoleIo.ReadLine("Enter your business name, e.g. McDonalds");
                while (string.IsNullOrEmpty(merchantName))
                    merchantName = ConsoleIo.ReadLine("Enter your outlet name, e.g. McDonalds Fleet Street");
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
            
            if (_truModule.IsActivated())
            {
                PinPad.DisplayMessage("This device is activated");
            }
            else
            {
                PinPad.DisplayMessage("This device is not activated");
            }
        }
        public override void Example()
        {
            _consoleIo.WriteLine(ConsoleColor.Gray, "Payment Application: About to make a payment");
            _truModule.DoRating();
            _consoleIo.WriteLine(ConsoleColor.Gray, "Payment Application: Sending Transaction");
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
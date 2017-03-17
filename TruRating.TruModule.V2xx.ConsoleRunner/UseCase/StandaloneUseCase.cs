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
        private readonly IConsoleWriter _consoleWriter;
        private TruModuleStandalone _truModule;

        public StandaloneUseCase(IConsoleWriter consoleWriter, IConsoleSettings consoleSettings, IDevice device)
            : base(consoleWriter, consoleSettings, device)
        {
            _consoleWriter = consoleWriter;
            _consoleSettings = consoleSettings;
        }

        public override void Init()
        {
            var truServiceClient = new TruServiceClient<Request, Response>(_consoleSettings.HttpTimeoutMs,
                _consoleSettings.TruServiceUrl,
                new MacSignatureCalculator(_consoleSettings.TransportKey, _consoleWriter), _consoleWriter);
            _truModule = new TruModuleStandalone(Device, truServiceClient, _consoleWriter,
                new TruServiceMessageFactory(), _consoleSettings);
            if (!_truModule.IsActivated())
            {
                _consoleWriter.WriteLine(ConsoleColor.Gray,
                    "Standalone UseCase: Not activated at start-up, prompting registration");
                _truModule.Activate();
            }
        }

        public override void Example()
        {
            _consoleWriter.WriteLine(ConsoleColor.Gray, "Payment Application: About to make a payment");
            _truModule.DoRating();
            _consoleWriter.WriteLine(ConsoleColor.Gray, "Payment Application: Sending Transaction");
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
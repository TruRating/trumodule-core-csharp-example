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
using TruRating.TruModule.V2xx.Messages;
using TruRating.TruModule.V2xx.Network;
using TruRating.TruModule.V2xx.Security;

namespace TruRating.TruModule.V2xx.ConsoleRunner.UseCase
{
    public class SemiIntegratedUseCase : UseCaseBase
    {
        private readonly IConsoleSettings _consoleSettings;
        private readonly IConsoleIo _consoleIo;
        private ITruModuleSemiIntegrated _truModule;

        public SemiIntegratedUseCase(IConsoleIo consoleIo, IConsoleSettings consoleSettings,
            IDevice device, IReceiptManager receiptManager) : base(consoleIo, consoleSettings, device, receiptManager)
        {
            _consoleIo = consoleIo;
            _consoleSettings = consoleSettings;
        }

        public override void Init()
        {
            var truServiceClient = TruServiceHttpClient.CreateDefault(_consoleSettings.HttpTimeoutMs,
                _consoleSettings.TruServiceUrl,_consoleIo,
                new MacSignatureCalculator(_consoleSettings.TransportKey, _consoleIo));
            _truModule = new TruModuleSemiIntegrated(Device, ReceiptManager,truServiceClient, _consoleIo,
                new TruServiceMessageFactory(), _consoleSettings);
        }

        public override void Example()
        {
            _consoleIo.WriteLine(ConsoleColor.Gray, "Payment Application: About to make a payment");
            _truModule.DoRating();
            var posEvent = CreateRequestPosEventList();
            _truModule.SendBatchedPosEvents(posEvent);
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

        public RequestPosEventList CreateRequestPosEventList()
        {
            return new RequestPosEventList
            {
                StartTransaction = CreateRequestPosStartTransaction(),
                StartTilling = CreateRequestPosStartTilling(),
                Items = new object[] {CreateRequestPosItem()},
                EndTilling = CreateRequestPosEndTilling(),
                EndTransaction = CreateRequestPosEndTransaction()
            };
        }

        private RequestPosStartTilling CreateRequestPosStartTilling()
        {
            var posEvent = new RequestPosStartTilling();
            return posEvent;
        }

        private RequestPosStartTransaction CreateRequestPosStartTransaction()
        {
            var posEvent = new RequestPosStartTransaction
            {
                AttendantType = AttendantType.ATTENDED,
                AttendantTypeSpecified = true,
                OperatorId = "James",
                SalesPersonId = "James",
                TillType = "SCO"
            };
            return posEvent;
        }

        private RequestPosEndTilling CreateRequestPosEndTilling()
        {
            var posEvent = new RequestPosEndTilling
            {
                SubTotalAmount = 640
            };
            return posEvent;
        }

        private RequestPosEndTransaction CreateRequestPosEndTransaction()
        {
            var posEvent = new RequestPosEndTransaction();
            return posEvent;
        }

        private RequestPosItem CreateRequestPosItem()
        {
            var posEvent = new RequestPosItem
            {
                Quantity = 1
            };
            return posEvent;
        }

        public override bool IsApplicable()
        {
            return _consoleSettings.PosIntegration == PosIntegration.Semi;
        }
    }
}
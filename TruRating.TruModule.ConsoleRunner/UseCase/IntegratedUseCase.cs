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
using System.Threading;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.ConsoleRunner.Environment;
using TruRating.TruModule.Device;
using TruRating.TruModule.Messages;

namespace TruRating.TruModule.ConsoleRunner.UseCase
{
    public class IntegratedUseCase : UseCaseBase
    {
        private readonly IConsoleSettings _consoleSettings;
        private readonly IConsoleIo _consoleIo;
        private TruModuleIntegrated _truModule;

        public IntegratedUseCase(IConsoleIo consoleIo, IConsoleSettings consoleSettings, IDevice device, IReceiptManager receiptManager)
            : base(consoleIo, consoleSettings, device, receiptManager)
        {
            _consoleIo = consoleIo;
            _consoleSettings = consoleSettings;
        }

        public override void Init()
        {
            _truModule = new TruModuleIntegrated(_consoleIo,_consoleSettings, Device, ReceiptManager);
        }

        public override void Example()
        {
            var log = new List<Type>();
            var sessionId = Guid.NewGuid().ToString();
            _consoleIo.WriteLine(ConsoleColor.Gray, "Pos Application: starting tilling");
            var posParams = new PosParams
            {
                SessionId = sessionId,
                PartnerId = _consoleSettings.PartnerId,
                MerchantId = _consoleSettings.MerchantId,
                TerminalId = _consoleSettings.TerminalId,
                Url = _consoleSettings.TruServiceUrl
            };
            while (true) //Send all pos events
            {
                var posEvent = CreateRequestPosEvent(sessionId, log);
                if (posEvent == null)
                {
                    break;
                }
                _truModule.SendPosEvent(posParams, posEvent);
                Thread.Sleep(250); //Wait a short length of time before continuing
            }
            _consoleIo.WriteLine(ConsoleColor.Gray, "Pos Application: About to make a payment");
            _truModule.InitiatePayment(posParams);
            _consoleIo.WriteLine(ConsoleColor.Gray, "Pos Application: customer made payment");
            Thread.Sleep(250); //Wait a short length of time between Making a payment and transation
            _consoleIo.WriteLine(ConsoleColor.Gray, "Pos Application: Payment complete");
            _truModule.SendTransaction(posParams, new RequestTransaction
            {
                Amount = Rand.Next(1000, 2000),
                Currency = 826,
                DateTime = DateTime.UtcNow,
                Gratuity = 0,
                Id = Guid.NewGuid().ToString(),
                Result = TransactionResult.APPROVED
            });
        }

        public RequestPosEvent CreateRequestPosEvent(string sessionId, ICollection<Type> log)
        {
            object posEvent = null;
            //Make all pos events
            //This creates a single pos event of each type in order - this is the minimum number of events expected by TruService
            if (!log.Contains(typeof (RequestPosStartTransaction)))
            {
                posEvent = CreateRequestPosStartTransaction();
            }
            else if (!log.Contains(typeof (RequestPosStartTilling)))
            {
                posEvent = CreateRequestPosStartTilling();
            }
            else if (!log.Contains(typeof (RequestPosItem)))
            {
                posEvent = CreateRequestPosItem();
            }
            else if (!log.Contains(typeof (RequestPosEndTilling)))
            {
                posEvent = CreateRequestPosEndTilling();
            }
            else if (!log.Contains(typeof (RequestPosEndTransaction)))
            {
                posEvent = CreateRequestPosEndTransaction();
            }
            if (posEvent != null)
            {
                log.Add(posEvent.GetType());
                return new RequestPosEvent {Items = new[] {posEvent}};
            }
            return null;
        }

        public override bool IsApplicable()
        {
            return _consoleSettings.PosIntegration == PosIntegration.Integrated;
        }

        private static RequestPosStartTilling CreateRequestPosStartTilling()
        {
            var posEvent = new RequestPosStartTilling();
            return posEvent;
        }

        private static RequestPosStartTransaction CreateRequestPosStartTransaction()
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

        private static RequestPosEndTilling CreateRequestPosEndTilling()
        {
            var posEvent = new RequestPosEndTilling
            {
                SubTotalAmount = 640
            };
            return posEvent;
        }

        private static RequestPosEndTransaction CreateRequestPosEndTransaction()
        {
            var posEvent = new RequestPosEndTransaction();
            return posEvent;
        }

        private static RequestPosItem CreateRequestPosItem()
        {
            var posEvent = new RequestPosItem
            {
                Quantity = 1
            };
            return posEvent;
        }
    }
}
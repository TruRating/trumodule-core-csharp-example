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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Device;
using TruRating.TruModule.V2xx.Messages;
using TruRating.TruModule.V2xx.Network;
using TruRating.TruModule.V2xx.Settings;
using TruRating.TruModule.V2xx.Util;

namespace TruRating.TruModule.V2xx.Tests.Unit.TruModuleStandaloneTests.ActivateWithMerchantInfo
{
    public class TruModuleStanadaloneActivateWithMerchantInfoTestContext: MsTestsContext<TruModuleStandalone>
    {
        protected ITruServiceMessageFactory TruServiceMessageFactory { get; set; }
        protected ITruServiceClient TruServiceClient { get; set; }
        protected ISettings Settings { get; set; }

        protected Request Request;
        protected Response Response;
        protected int ActivationReCheckTimeMinutes = 60 * 24 * 365;

        [TestInitialize]
        public void Setup()
        {
            DateTimeProvider.UtcNow = new DateTime(2000, 01, 01);

            Request = new Request()
            {
                Item = new ResponseStatus()
            };

            TruServiceMessageFactory = MockOf<ITruServiceMessageFactory>();
            TruServiceMessageFactory.Stub(
                x =>
                    x.AssembleRequestActivate(Arg<RequestParams>.Is.Anything, Arg<IDevice>.Is.Anything, Arg<IReceiptManager>.Is.Anything,
                        Arg<int>.Is.Anything, Arg<string>.Is.Anything,
                        Arg<PaymentInstant>.Is.Anything,Arg<string>.Is.Anything, Arg<string>.Is.Anything, 
                        Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, 
                        Arg<string>.Is.Anything)).Return(Request);
            
            TruServiceClient = MockOf<ITruServiceClient>();
            TruServiceClient.Stub(t => t.Send(Arg<Request>.Is.Anything)).Return(Response);

            Settings = MockOf<ISettings>();
            
        }

        protected Response CreateResponseStatus(bool isActive, int activationReCheckTimeMinutes
)
        {
            return new Response()
            {
                Item = new ResponseStatus()
                {
                    IsActive = isActive,
                    TimeToLive = activationReCheckTimeMinutes
                }
            };

        }
    }
}
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
using Rhino.Mocks;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.Device;
using TruRating.TruModule.Messages;
using TruRating.TruModule.Network;
using TruRating.TruModule.Settings;

namespace TruRating.TruModule.Tests.Unit.TruModuleTests
{
    public class TestContextTruModuleSemiIntegrated : TruModuleSemiIntegrated
    {
        //Modify visibility for Unit Tests
        public TestContextTruModuleSemiIntegrated(ILogger logger, ISettings settings, IDevice device, IReceiptManager receiptManager, ITruServiceClient truServiceClient, ITruServiceMessageFactory truServiceMessageFactory) : base(logger, settings, device, receiptManager, truServiceClient, truServiceMessageFactory)
        {
        }
    }
    public class TestContextTruModuleIntegrated : TruModuleIntegrated
    {
        //Modify visibility for Unit Tests
        public TestContextTruModuleIntegrated(ILogger logger, ISettings settings, IDevice device, IReceiptManager receiptManager, ITruServiceClient truServiceClient, ITruServiceMessageFactory truServiceMessageFactory) : base(logger, settings, device, receiptManager, truServiceClient, truServiceMessageFactory)
        {
        }
    }

    public class TestContextTruModuleStandalone : TruModuleStandalone
    {
        //Modify visibility for Unit Tests
        public TestContextTruModuleStandalone(ILogger logger, ISettings settings, IDevice device, IReceiptManager receiptManager, ITruServiceClient truServiceClient, ITruServiceMessageFactory truServiceMessageFactory) : base(logger, settings, device, receiptManager, truServiceClient, truServiceMessageFactory)
        {
        }
    }
    public class TestContextTruModule : TruModule
    {
        public TestContextTruModule(IDevice device, IReceiptManager receiptManager, ITruServiceClient truServiceClient,
            ILogger logger, ITruServiceMessageFactory truServiceMessageFactory, ISettings settings)
            : base(logger, settings, device, receiptManager, truServiceClient, truServiceMessageFactory)
        {
        }

        //Modify visibility for Unit Tests
        public new void DoRating(Request request)
        {
            base.DoRating(request);
        }
    }

    public abstract class TruModuleTestsContext : MsTestsContext<TestContextTruModule>
    {
        protected Request Request;
        protected Response Response;
        protected ITruServiceMessageFactory TruServiceMessageFactory;
        protected ISettings Settings;
        protected IDevice Device;
        protected ITruServiceClient TruServiceClient;

        public void SetupBase(Trigger trigger)
        {

            Request = new Request
            {
                PartnerId = "1",
                MerchantId = "1",
                TerminalId = "1",
                SessionId = "1",
                Item = new RequestQuestion
                {
                    Trigger = trigger,
                }
            };
            Response = new Response
            {
                Item =
                   new ResponseDisplay
                   {
                       Language =
                           new[]
                           {
                                new ResponseLanguage
                                {
                                    Rfc1766 = "en-GB",
                                    Question = new ResponseQuestion {Value = "Hello", TimeoutMs = 45000},
                                    Screen =
                                        new[]
                                        {
                                            new ResponseScreen {Value = "Thanks", When = When.RATED},
                                            new ResponseScreen {Value = "Sorry", When = When.NOTRATED},
                                        }
                                }
                           }
                   }
            };

            Settings = MockOf<ISettings>();
            Sut.Activated = true;
            TruServiceMessageFactory = MockOf<ITruServiceMessageFactory>();
            TruServiceMessageFactory.Stub(
                x =>
                    x.AssembleRequestQuestion(Arg<RequestParams>.Is.Anything, Arg<IDevice>.Is.Anything, Arg<IReceiptManager>.Is.Anything,Arg<Trigger>.Is.Anything)).Return(Request);

            Device = MockOf<IDevice>();

            Device.Stub(x => x.GetCurrentLanguage()).Return("en-GB");
            TruServiceClient = MockOf<ITruServiceClient>();
            TruServiceClient.Stub(x => x.Send(Arg<Request>.Is.Anything)).Return(Response);
        }
    }
}

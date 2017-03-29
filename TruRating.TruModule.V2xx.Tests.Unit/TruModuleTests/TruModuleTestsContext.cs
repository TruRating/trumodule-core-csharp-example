using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Device;
using TruRating.TruModule.V2xx.Messages;
using TruRating.TruModule.V2xx.Network;
using TruRating.TruModule.V2xx.Settings;
using TruRating.TruModule.V2xx.Util;

namespace TruRating.TruModule.V2xx.Tests.Unit.TruModuleTests
{
    //todo: find a better name. TruModuleDummy? TruModuleTestable?
    public class TroModuleNonAbstract : TruModule
    {
        public TroModuleNonAbstract(IDevice device, IReceiptManager receiptManager, ITruServiceClient truServiceClient, ILogger logger, ITruServiceMessageFactory truServiceMessageFactory, ISettings settings) : base(device, receiptManager, truServiceClient, logger, truServiceMessageFactory, settings)
        {
        }

        public new void DoRating(Request request)
        {
            base.DoRating(request);
        }
    }

    public abstract class TruModuleTestsContext : MsTestsContext<TroModuleNonAbstract>
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
            Settings.IsActivated = true;
            TruServiceMessageFactory = MockOf<ITruServiceMessageFactory>();
            TruServiceMessageFactory.Stub(
                x =>
                    x.AssembleRequestQuestion(Arg<IDevice>.Is.Anything, Arg<IReceiptManager>.Is.Anything,
                        Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything,
                        Arg<string>.Is.Anything, Arg<Trigger>.Is.Anything)).Return(Request);

            Device = MockOf<IDevice>();

            Device.Stub(x => x.GetCurrentLanguage()).Return("en-GB");
            TruServiceClient = MockOf<ITruServiceClient>();
            TruServiceClient.Stub(x => x.Send(Arg<Request>.Is.Anything)).Return(Response);
        }
    }
}

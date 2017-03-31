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
                    x.AssembleRequestActivate(Arg<IDevice>.Is.Anything, Arg<IReceiptManager>.Is.Anything,
                        Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything,
                        Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<string>.Is.Anything,
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
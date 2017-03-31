using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Device;
using TruRating.TruModule.V2xx.Messages;
using TruRating.TruModule.V2xx.Network;

namespace TruRating.TruModule.V2xx.Tests.Unit.TruModuleStandaloneTests.Activate
{
    public class TruModuleStanadaloneActivateWithRegCodeTestContext: MsTestsContext<TruModuleStandalone>
    {
        protected ITruServiceMessageFactory TruServiceMessageFactory { get; set; }
        protected ITruServiceClient TruServiceClient { get; set; }

        protected Request Request;
        protected Response Response;
        protected string RegistrationCode;

        [TestInitialize]
        public void Setup()
        {
            Request = new Request()
            {
                Item = new ResponseStatus()
            };

            TruServiceMessageFactory = MockOf<ITruServiceMessageFactory>();
            TruServiceMessageFactory.Stub(
                x =>
                    x.AssembleRequestQuestion(Arg<IDevice>.Is.Anything, Arg<IReceiptManager>.Is.Anything,
                        Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything,
                        Arg<string>.Is.Anything, Arg<Trigger>.Is.Anything)).Return(Request);

            TruServiceClient = MockOf<ITruServiceClient>();
            TruServiceClient.Stub(t => t.Send(Arg<Request>.Is.Anything)).Return(Response);

            RegistrationCode = "ABCD";
        }

        protected Response CreateResponseStatus(bool isActive)
        {
            return new Response()
            {
                Item = new ResponseStatus()
                {
                    IsActive = isActive
                }
            };

        }
    }
}
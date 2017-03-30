using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Device;
using TruRating.TruModule.V2xx.Settings;

namespace TruRating.TruModule.V2xx.Tests.Unit.TruModuleStandaloneTests
{
    [TestClass]
    public class WhenSendingATransaction : ActiveTruModuleStandaloneTestsContext
    {
        [TestMethod]
        public void TruServiceShouldBeContactedIfActive()
        {
            MockOf<ISettings>().IsActivated = true;

            Sut.SendTransaction(new RequestTransaction());

            TruServiceClient.AssertWasCalled( t => t.Send(Arg<Request>.Is.Anything));
        }

        [TestMethod]
        public void TruServiceShouldNotBeContactedIfNotActive()
        {
            MockOf<ISettings>().IsActivated = false;

            Sut.SendTransaction(new RequestTransaction());

            TruServiceClient.AssertWasNotCalled(t => t.Send(Arg<Request>.Is.Anything)); //Todo: This should pass. check with Toby.
        }
        

        public WhenSendingATransaction() : base(Trigger.DWELLTIMEEXTEND)
        {
        }
    }
}
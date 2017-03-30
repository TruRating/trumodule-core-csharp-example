using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using TruRating.Dto.TruService.V220;

namespace TruRating.TruModule.V2xx.Tests.Unit.TruModuleStandaloneTests
{
    [TestClass]
    public class WhenActivating : ActiveTruModuleStandaloneTestsContext
    {
        [TestMethod]
        public void TruServiceShouldBeContactedIfActive()
        {
            Settings.IsActivated = true;

            Sut.SendTransaction(new RequestTransaction());

            TruServiceClient.AssertWasCalled(t => t.Send(Arg<Request>.Is.Anything));
        }

        [TestMethod]
        public void TruServiceShouldNotBeContactedIfNotActive()
        {
            Settings.IsActivated = false;

            Sut.SendTransaction(new RequestTransaction());

            TruServiceClient.AssertWasCalled(t => t.Send(Arg<Request>.Is.Anything), options => options.Repeat.Once()); //Todo: This should pass. check with Toby.
        }


        public WhenActivating() : base(Trigger.DWELLTIMEEXTEND)
        {
        }
    }
}
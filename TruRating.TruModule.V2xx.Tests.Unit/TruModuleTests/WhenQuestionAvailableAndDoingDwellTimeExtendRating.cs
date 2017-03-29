using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Device;

namespace TruRating.TruModule.V2xx.Tests.Unit.TruModuleTests
{

    [TestClass]
    public class WhenQuestionAvailableAndDoingDwellTimeExtendRating : TruModuleTestsContext
    {
        [TestInitialize]
        public void Setup()
        {
            base.SetupBase(Trigger.DWELLTIMEEXTEND);
            Device.Stub(x => x.Display1AQ1KR(Arg<string>.Is.Anything, Arg<int>.Is.Anything)).WhenCalled(invocation =>
            {
                Thread.Sleep(250);
            }).Return(2);
            Sut.DoRating(Request);
        }
       
        [TestMethod]
        public void ItShouldSendThreeRequests()
        {
            //Once for TruModule init and 2 for Question and Rating
            TruServiceClient.AssertWasCalled(x => x.Send(Arg<Request>.Is.Anything), options => options.Repeat.Times(3));
        }

        [TestMethod]
        public void ItShouldCall1AQ1KRWithMaxTimeout()
        {
            Device.AssertWasCalled(x => x.Display1AQ1KR("Hello", int.MaxValue));
        }

        [TestMethod]
        public void ItShouldSedndARating()
        {
            var arguments = TruServiceMessageFactory.GetArgumentsForCallsMadeOn(
                x => x.AssembleRequestRating(Arg<Request>.Is.Anything, Arg<RequestRating>.Is.Anything))[0];
            Assert.IsTrue(((RequestRating)arguments[1]).Value == 2);
        }
     
    }
}
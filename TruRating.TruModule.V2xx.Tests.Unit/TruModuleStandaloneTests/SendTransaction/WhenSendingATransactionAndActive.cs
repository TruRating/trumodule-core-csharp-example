using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Network;
using TruRating.TruModule.V2xx.Settings;

namespace TruRating.TruModule.V2xx.Tests.Unit.TruModuleStandaloneTests
{
    [TestClass]
    public class WhenSendingATransactionAndActive : MsTestsContext<TruModuleStandalone>
    {
        [TestInitialize]
        public void Setup()
        {
            MockOf<ISettings>().IsActivated = false;
            MockOf<ISettings>().ActivationRecheck = DateTime.MaxValue;

            Sut.SendTransaction(new RequestTransaction());

        }

        [TestMethod]
        public void TruServiceShouldBeCalled()
        {
            MockOf<ITruServiceClient>().AssertWasCalled( t => t.Send(Arg<Request>.Is.Anything) );
        }
      
    }
}
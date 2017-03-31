using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Messages;
using TruRating.TruModule.V2xx.Network;
using TruRating.TruModule.V2xx.Settings;

namespace TruRating.TruModule.V2xx.Tests.Unit.TruModuleStandaloneTests.SendTransaction
{
    [TestClass]
    public class WhenSendingATransactionAndNotActive : MsTestsContext<TruModuleStandalone>
    {
        private Request objToReturn;

        [TestInitialize]
        public void Setup()
        {
            MockOf<ISettings>().IsActivated = false;
            MockOf<ISettings>().ActivationRecheck = DateTime.MaxValue;
            objToReturn = new Request();
            MockOf<ITruServiceMessageFactory>().Stub(x => x.AssembleRequestTransaction(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<RequestTransaction>.Is.Anything)).Return(objToReturn);
            Sut.SendTransaction(new RequestTransaction());

        }
        [TestMethod]
        public void TruServiceShouldNotBeCalled()
        {
            
            MockOf<ITruServiceClient>().AssertWasNotCalled(t => t.Send(objToReturn)); 

        }
         
    }
}
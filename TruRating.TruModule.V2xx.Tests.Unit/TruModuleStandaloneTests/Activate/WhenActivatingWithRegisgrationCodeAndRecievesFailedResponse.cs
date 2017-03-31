using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TruRating.TruModule.V2xx.Tests.Unit.TruModuleStandaloneTests.Activate
{
    [TestClass]
    public class WhenActivatingWithRegisgrationCodeAndRecievesFailedResponse : TruModuleStanadaloneActivateWithRegCodeTestContext
    {
        private bool _isActivated;

        [TestInitialize]
        public void Setup()
        {
            Response = CreateResponseStatus(isActive: false);
            base.Setup();

            _isActivated = Sut.Activate(RegistrationCode);
        }

        [TestMethod]
        public void ActivationShouldBeSuccessful()
        {
            
            Assert.IsFalse(_isActivated);
        }
    }
}

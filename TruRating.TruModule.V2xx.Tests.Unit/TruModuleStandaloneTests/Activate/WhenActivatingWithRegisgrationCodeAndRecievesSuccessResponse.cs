using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TruRating.TruModule.V2xx.Tests.Unit.TruModuleStandaloneTests.Activate
{
    [TestClass]
    public class WhenActivatingWithRegisgrationCodeAndRecievesSuccessResponse : TruModuleStanadaloneActivateWithRegCodeTestContext
    {
        private bool _isActivated;

        [TestInitialize]
        public void Setup()
        {
            Response = CreateResponseStatus(isActive: true);
            base.Setup();

            _isActivated = Sut.Activate(RegistrationCode);
        }

        [TestMethod]
        public void ActivationShouldBeSuccessful()
        {
            Assert.IsTrue(_isActivated);
        }
    }
}
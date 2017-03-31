using Microsoft.VisualStudio.TestTools.UnitTesting;
using TruRating.TruModule.V2xx.Util;

namespace TruRating.TruModule.V2xx.Tests.Unit.TruModuleStandaloneTests.ActivateWithRegistraionCode
{
    [TestClass]
    public class WhenActivatingWithRegisgrationCodeAndRecievesFailedResponse : TruModuleStanadaloneActivateWithRegCodeTestContext
    {
        private bool _isActivated;

        [TestInitialize]
        public void Setup()
        {
            Response = CreateResponseStatus(isActive: false, activationReCheckTimeMinutes:ActivationReCheckTimeMinutes);
            base.Setup();

            _isActivated = Sut.Activate(RegistrationCode);
        }

        [TestMethod]
        public void ActivationShouldBeSuccessful()
        {
            
            Assert.IsFalse(_isActivated);
        }

        [TestMethod]
        public void ActivationReCheckTimeShouldBeSet()
        {
            Assert.AreEqual(DateTimeProvider.UtcNow.AddMinutes(ActivationReCheckTimeMinutes), Settings.ActivationRecheck);

        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Util;

namespace TruRating.TruModule.V2xx.Tests.Unit.TruModuleStandaloneTests.ActivateWithMerchantInfo
{
    [TestClass]
    public class WhenActivatingWithMerchantInfoAndRecievesFailedResponse : TruModuleStanadaloneActivateWithMerchantInfoTestContext
    {
        private bool _isActivated;

        [TestInitialize]
        public void Setup()
        {
            Response = CreateResponseStatus(isActive: false, activationReCheckTimeMinutes:ActivationReCheckTimeMinutes);
            base.Setup();

            _isActivated = Sut.Activate(100,"TimeZone", PaymentInstant.PAYAFTER, "abc@email.com","password","address",
                        "mobile", "merchant name", "business name");
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

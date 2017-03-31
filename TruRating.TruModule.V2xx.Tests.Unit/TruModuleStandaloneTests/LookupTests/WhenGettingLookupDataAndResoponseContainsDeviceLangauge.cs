using Microsoft.VisualStudio.TestTools.UnitTesting;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Tests.Unit.TruModuleStandaloneTests.LookupTests;

namespace TruRating.TruModule.V2xx.Tests.Unit.TruModuleStandaloneTests
{
    [TestClass]
    public class WhenGettingLookupDataAndResoponseContainsDeviceLangauge: TruModuleStandaloneLookupTestContext
    {
       

        [TestInitialize]
        public void Setup()
        {
            ResponseLanguageCode = "en";
            DeviceLanguageCode = "en";

            base.Setup();

            LookupsReturned = Sut.GetLookups(LookupName.SECTORNODE);
        }

        [TestMethod]
        public void ItShouldReturnLookupData()
        {
            CollectionAssert.AreEqual(LookupsReturned, ResponseLookupLanguage.Option);
        }

    }
}
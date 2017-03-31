﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Tests.Unit.TruModuleStandaloneTests.LookupTests;

namespace TruRating.TruModule.V2xx.Tests.Unit.TruModuleStandaloneTests
{
    [TestClass]
    public class WhenGettingLookupDataAndResoponseDoesNotContainDeviceLangauge : TruModuleStandaloneLookupTestContext
    {

        [TestInitialize]
        public void Setup()
        {
            ResponseLanguageCode = "en";
            DeviceLanguageCode = "fr";

            base.Setup();

            LookupsReturned = Sut.GetLookups(LookupName.SECTORNODE);
        }

        [TestMethod]
        public void ItShouldNotReturnLookupData()
        {
            Assert.IsNull(LookupsReturned);
        }
    }
}
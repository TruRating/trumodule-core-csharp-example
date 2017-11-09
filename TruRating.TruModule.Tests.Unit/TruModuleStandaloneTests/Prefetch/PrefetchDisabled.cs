using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TruRating.Dto.TruService.V220;
using Rhino.Mocks;

namespace TruRating.TruModule.Tests.Unit.TruModuleStandaloneTests.Prefetch
{
    [TestClass]
    public class PrefetchDisabled : ActiveTruModuleStandaloneTestsContext
    {
        [TestInitialize]
        public void Setup()
        {
            Settings.UsePrefetch = false;
        }

        [TestMethod]
        public void QuestionFetchedBeforeRatingSent()
        {
            TruServiceClient.GetMockRepository().Ordered();
            TruServiceClient.Expect(x => x.Send(Arg<Request>.Matches(r => r.Item is RequestQuestion)));
            TruServiceClient.Expect(x => x.Send(Arg<Request>.Matches(r => r.Item is RequestRating)));

            Sut.DoRating();
            TruServiceClient.VerifyAllExpectations();
        }

        [TestMethod]
        public void QuestionNotFetchedAfterTransactionSent()
        {
            Sut.SendTransaction(new RequestTransaction());
            TruServiceClient.AssertWasNotCalled(x => x.Send(Arg<Request>.Matches(r => r.Item is RequestQuestion)));
        }

        public PrefetchDisabled():base(Dto.TruService.V220.Trigger.DWELLTIME)
        {

        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TruRating.Dto.TruService.V220;
using Rhino.Mocks;
using System.Threading;

namespace TruRating.TruModule.Tests.Unit.TruModuleStandaloneTests.Prefetch
{
    [TestClass]
    public class PrefetchEnabled : ActiveTruModuleStandaloneTestsContext
    {
        [TestInitialize]
        public void Setup()
        {
            Settings.UsePrefetch = true;
        }

        [TestMethod]
        public void QuestionNotFetchedBeforeRating()
        {
            Sut.DoRating();
            TruServiceClient.AssertWasNotCalled(x => x.Send(Arg<Request>.Matches(r => r.Item is RequestQuestion)));
        }

        [TestMethod]
        public void QuestionFetchedAfterTransactionSent()
        {
            Sut.SendTransaction(new RequestTransaction());
            Thread.Sleep(500);//wait for the background send.
            TruServiceClient.AssertWasCalled(x => x.Send(Arg<Request>.Matches(r => r.Item is RequestQuestion)));
        }

        public PrefetchEnabled():base(Dto.TruService.V220.Trigger.DWELLTIME)
        {

        }
    }
}

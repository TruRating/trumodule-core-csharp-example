// The MIT License
// 
// Copyright (c) 2017 TruRating Ltd. https://www.trurating.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Environment;
using TruRating.TruModule.V2xx.Messages;
using TruRating.TruModule.V2xx.Scenarios;

namespace TruRating.TruModule.V2xx.Tests.Unit.Scenarios
{
    [TestClass]
    public class V200ExemplarMsTests : MsTestsContext<V200Exemplar>
    {
        private readonly Response _responseDisplay;

        public V200ExemplarMsTests()
        {
            _responseDisplay = new Response
            {
                Item = new ResponseDisplay
                {
                    Language = new[]
                    {
                        new ResponseLanguage
                        {
                            Question = new ResponseQuestion {TimeoutMs = 500, Value = "Hello World"},
                            Receipt = new[]
                            {
                                new ResponseReceipt
                                {
                                    When = When.RATED,
                                    Type = ReceiptType.CUSTOMER,
                                    Value = "Thanks for rating"
                                },
                                new ResponseReceipt
                                {
                                    When = When.NOTRATED,
                                    Type = ReceiptType.CUSTOMER,
                                    Value = "Sorry you didn't rate"
                                }
                            },
                            Rfc1766 = "en-GB",
                            Screen = new[]
                            {
                                new ResponseScreen {TimeoutMs = 1, When = When.RATED, Value = "Thanks for rating"},
                                new ResponseScreen
                                {
                                    TimeoutMs = 1,
                                    When = When.NOTRATED,
                                    Value = "Sorry you didn't rate"
                                }
                            }
                        }
                    }
                }
            };
        }

        [TestMethod]
        public void IsApplicableToV200MessagesOnly()
        {
            MockOf<ISettings>().TsiVersion = TsiVersion.V200;
            Assert.IsTrue(Sut.IsApplicable());
        }

        [TestMethod]
        public void ShouldSendTransactionWhenNoQuestionAvailable()
        {
            MockOf<ISettings>().Languages = new[] {"en-GB"};
            MockOf<ITsiV200Messages>()
                .Stub(
                    x =>
                        x.SendRequestQuestion(Arg<List<RequestLanguage>>.Is.Anything, Arg<string>.Is.Anything,
                            Arg<Trigger>.Is.Anything))
                .Return(new Response {Item = null});
            Sut.Scenario();
            MockOf<ITsiV200Messages>().AssertWasCalled(x => x.SendRequestTransaction(Arg<string>.Is.Anything));
        }

        [TestMethod]
        public void ShouldCaptureRatingWhenQuestionAvailableAndUserRates()
        {
            MockOf<ISettings>().Languages = new[] {"en-GB"};
            MockOf<IDevice>()
                .Stub(x => x.Display1AQ1KR(Arg<string>.Is.Anything, Arg<int>.Is.Anything))
                .Return('5');
            MockOf<ITsiV200Messages>()
                .Stub(
                    x =>
                        x.SendRequestQuestion(Arg<List<RequestLanguage>>.Is.Anything, Arg<string>.Is.Anything,
                            Arg<Trigger>.Is.Anything))
                .Return(
                    _responseDisplay);
            Sut.Scenario();
            MockOf<ITsiV200Messages>()
                .AssertWasCalled(x => x.SendRequestRating(Arg<string>.Is.Anything, Arg<RequestRating>.Is.Anything));
            MockOf<ITsiV200Messages>().AssertWasCalled(x => x.SendRequestTransaction(Arg<string>.Is.Anything));
            MockOf<IDevice>().AssertWasCalled(x => x.Display1AQ1KR("Hello World",500));
            MockOf<IDevice>().AssertWasCalled(x => x.Display1AQ1KR("Thanks for rating",1));
            MockOf<IDevice>().AssertWasCalled(x => x.PrintReceipt("Thanks for rating"));
        }

        [TestMethod]
        public void ShouldCaptureRatingWhenQuestionAvailableAndUserSkips()
        {
            MockOf<ISettings>().Languages = new[] {"en-GB"};
            MockOf<ITsiV200Messages>()
                .Stub(
                    x =>
                        x.SendRequestQuestion(Arg<List<RequestLanguage>>.Is.Anything, Arg<string>.Is.Anything,
                            Arg<Trigger>.Is.Anything))
                .Return(
                    _responseDisplay);
            Sut.Scenario();
            MockOf<ITsiV200Messages>()
                .AssertWasCalled(x => x.SendRequestRating(Arg<string>.Is.Anything, Arg<RequestRating>.Is.Anything));
            MockOf<ITsiV200Messages>().AssertWasCalled(x => x.SendRequestTransaction(Arg<string>.Is.Anything));
            MockOf<IDevice>().AssertWasCalled(x => x.Display1AQ1KR("Hello World",500));
            MockOf<IDevice>().AssertWasCalled(x => x.Display1AQ1KR("Sorry you didn't rate",1));
            MockOf<IDevice>().AssertWasCalled(x => x.PrintReceipt("Sorry you didn't rate"));
        }

        [TestMethod]
        public void ShouldCaptureRatingWhenQuestionAvailableAndUserTimesOut()
        {
            MockOf<ISettings>().Languages = new[] {"en-GB"};
            MockOf<IDevice>().Stub(x => x.Display1AQ1KR(Arg<string>.Is.Anything, Arg<int>.Is.Anything)).Throw(new TimeoutException());
            MockOf<ITsiV200Messages>()
                .Stub(
                    x =>
                        x.SendRequestQuestion(Arg<List<RequestLanguage>>.Is.Anything, Arg<string>.Is.Anything,
                            Arg<Trigger>.Is.Anything))
                .Return(_responseDisplay);
            Sut.Scenario();
            MockOf<ITsiV200Messages>()
                .AssertWasCalled(x => x.SendRequestRating(Arg<string>.Is.Anything, Arg<RequestRating>.Is.Anything));
            MockOf<ITsiV200Messages>().AssertWasCalled(x => x.SendRequestTransaction(Arg<string>.Is.Anything));
            MockOf<IDevice>().AssertWasCalled(x => x.Display1AQ1KR("Hello World",500));
            MockOf<IDevice>().AssertWasCalled(x => x.Display1AQ1KR("Sorry you didn't rate",1));
            MockOf<IDevice>().AssertWasCalled(x => x.PrintReceipt("Sorry you didn't rate"));
        }

        [TestMethod]
        public void ShouldCaptureRatingWhenQuestionAvailableAndNotInUsersLanguage()
        {
            MockOf<ISettings>().Languages = new[] {"en-FR"};
            MockOf<IDevice>().Stub(x => x.Display1AQ1KR(Arg<string>.Is.Anything, Arg<int>.Is.Anything)).Throw(new TimeoutException());
            MockOf<ITsiV200Messages>()
                .Stub(
                    x =>
                        x.SendRequestQuestion(Arg<List<RequestLanguage>>.Is.Anything, Arg<string>.Is.Anything,
                            Arg<Trigger>.Is.Anything))
                .Return(_responseDisplay);
            Sut.Scenario();
            MockOf<ITsiV200Messages>()
                .AssertWasCalled(x => x.SendRequestRating(Arg<string>.Is.Anything, Arg<RequestRating>.Is.Anything));
            MockOf<ITsiV200Messages>().AssertWasCalled(x => x.SendRequestTransaction(Arg<string>.Is.Anything));
            MockOf<IDevice>().AssertWasNotCalled(x => x.DisplayMessage(Arg<string>.Is.Anything));
            MockOf<IDevice>().AssertWasNotCalled(x => x.DisplayMessage(Arg<string>.Is.Anything));
            MockOf<IDevice>().AssertWasNotCalled(x => x.PrintReceipt(Arg<string>.Is.Anything));
        }
    }
}
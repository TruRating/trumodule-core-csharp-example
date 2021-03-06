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
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.Device;
using TruRating.TruModule.Messages;

namespace TruRating.TruModule.Tests.Unit.TruModuleTests
{

    [TestClass]
    public class WhenQuestionAvailableAndDoingDwellTimeExtendRating : TruModuleTestsContext
    {
        [TestInitialize]
        public void Setup()
        {
            base.SetupBase(Trigger.DWELLTIMEEXTEND);
            Device.Stub(x => x.Display1AQ1KR(Arg<string>.Is.Anything, Arg<int>.Is.Anything)).WhenCalled(invocation =>
            {
                Thread.Sleep(250);
            }).Return(2);

            Sut.DoRating();
        }
       
        [TestMethod]
        public void ItShouldSendThreeRequests()
        {
            //Once for TruModule init and 2 for Question and Rating
            TruServiceClient.AssertWasCalled(x => x.Send(Arg<Request>.Is.Anything), options => options.Repeat.Times(3));
        }

        [TestMethod]
        public void ItShouldCall1AQ1KRWithMaxTimeout()
        {
            Device.AssertWasCalled(x => x.Display1AQ1KR("Hello", int.MaxValue));
        }

        [TestMethod]
        public void ItShouldSendARating()
        {
            TruServiceClient.AssertWasCalled(x => x.Send(Arg<Request>.Matches(r => r.Item is RequestRating)));
            //not sure why we are getting all the calls back here, but we want the last one.
            var arguments = TruServiceClient.GetArgumentsForCallsMadeOn(x => x.Send(Arg<Request>.Matches(r => r.Item is RequestRating)))[2];
            Assert.AreEqual((((Request)arguments[0]).Item as RequestRating).Value , 2);
        }
        [TestMethod]
        public void ItShouldCallDisplayAcknowledgement()
        {
            var arguments =
                Device.GetArgumentsForCallsMadeOn(
                    x =>
                        x.DisplayAcknowledgement(Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<bool>.Is.Anything,
                            Arg<RatingContext>.Is.Anything))[0];
            Assert.IsTrue((bool)arguments[2] == true);
        }
     
    }
}
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.Network;
using TruRating.TruModule.Settings;
using TruRating.TruModule.Util;

namespace TruRating.TruModule.Tests.Unit.TruModuleTests.Activation
{
    [TestClass]
    public class WhenTruModuleIsInactiveAndTtlNotExpiredAndDoingAPassiveActivationCheck : TruModuleTestsContext
    {
        private bool _isActivated;
        private ITruServiceClient _truServiceClient;

        [TestInitialize]
        public void Setup()
        {
            DateTimeProvider.UtcNow = new DateTime(2000,01,01);
            Sut.ActivationRecheck= new DateTime(2001, 01,01);
            _truServiceClient = MockOf<ITruServiceClient>();

            _isActivated = Sut.IsActivated(bypassTruServiceCache:false); 
            
        }

        [TestMethod]
        public void ItShouldBeInactive()
        {
            Assert.IsFalse(_isActivated);
        }

        [TestMethod]
        public void ItShouldReturnIsActiveFalse()
        {
            _truServiceClient.AssertWasCalled(x => x.Send(Arg<Request>.Is.Anything), options => options.Repeat.Once());
        }
    }
}
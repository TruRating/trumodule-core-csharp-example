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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TruRating.TruModule.V2xx.Environment;

namespace TruRating.TruModule.V2xx.Tests.Unit.Enviroment
{
    [TestClass]
    public class MacSignatureCalculatorTests
    {
        [TestMethod]
        public void CalculateMacForKnownMessage()
        {
            var macSignatureCalculator = new MacSignatureCalculator();
            var result = macSignatureCalculator.Calculate("Super secret message", "000001002051431059683111") ==
                         "E133185A2953E98B978535CB9CEC1A691BCE247D5ABF17DCCC758E99A458AD780141F192E25B9BDD";
            Assert.IsTrue(result);
        }
    }
}
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
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Device;
using TruRating.TruModule.V2xx.Module;
using TruRating.TruModule.V2xx.Serialization;

namespace TruRating.TruModule.V2xx.Tests.Unit.Serialization
{
    [TestClass]
    public class TruServiceMessageFactoryTests :MsTestsContext<TruServiceMessageFactory>
    {
        [TestMethod]
        public void WhenAssemblyRequestQuery()
        {
            Assert.IsNotNull(Sut.AssemblyRequestQuery(MockOf<IDevice>(), MockOf<IReceiptManager>(), "", "", "", "", true));
        }

        [TestMethod]
        public void WhenAssembleRequestQuestion()
        {
            Assert.IsNotNull(Sut.AssembleRequestQuestion(MockOf<IDevice>(), MockOf<IReceiptManager>(), "", "", "", "",Trigger.CARDINSERTION));
        }

        [TestMethod]
        public void AssembleRequestTransaction()
        {
            Assert.IsNotNull(Sut.AssembleRequestTransaction( "", "", "", "", new RequestTransaction()));
        }

        [TestMethod]
        public void WhenAssembleRequestPosEvent()
        {
            Assert.IsNotNull(Sut.AssembleRequestPosEvent(new PosParams(), new RequestPosEvent()));
        }

        [TestMethod]
        public void WhenAssembleRequestPosEventList()
        {
            Assert.IsNotNull(Sut.AssembleRequestPosEvent(new PosParams(), new RequestPosEventList()));
        }

        [TestMethod]
        public void WhenAssembleRequestLookup()
        {
            Assert.IsNotNull(Sut.AssembleRequestLookup(MockOf<IDevice>(), MockOf<IReceiptManager>(), "", "", "", "",LookupName.SECTORNODE));
        }

        [TestMethod]
        public void WhenAssembleRequestActivate()
        {
            Assert.IsNotNull(Sut.AssembleRequestActivate(MockOf<IDevice>(), MockOf<IReceiptManager>(), "", "", "", "",0,"",PaymentInstant.PAYBEFORE,"","","","","",""));
        }

        [TestMethod]
        public void WhenAssembleRequestActivateCode()
        {
            Assert.IsNotNull(Sut.AssembleRequestActivate(MockOf<IDevice>(), MockOf<IReceiptManager>(), "", "", "", "",""));
        }
    }
}

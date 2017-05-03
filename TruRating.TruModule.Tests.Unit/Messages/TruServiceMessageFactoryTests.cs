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
using TruRating.TruModule.Device;
using TruRating.TruModule.Messages;

namespace TruRating.TruModule.Tests.Unit.Messages
{
    [TestClass]
    public class TruServiceMessageFactoryTests :MsTestsContext<TruServiceMessageFactory>
    {
        [TestMethod]
        public void ItShouldAssembleRequestRating()
        {
            Assert.IsNotNull(Sut.AssembleRequestRating(new Request(), new RequestRating()));
        }
        [TestMethod]
        public void ItShouldAssembleRequestQuery()
        {
            Assert.IsNotNull(Sut.AssemblyRequestQuery(MockOf<RequestParams>(), MockOf<IDevice>(), MockOf<IReceiptManager>(), true));
        }

        [TestMethod]
        public void ItShouldAssembleRequestQuestion()
        {
            Assert.IsNotNull(Sut.AssembleRequestQuestion(MockOf<RequestParams>(), MockOf<IDevice>(), MockOf<IReceiptManager>(),Trigger.CARDINSERTION));
        }

        [TestMethod]
        public void ItShouldAssembleRequestTransaction()
        {
            Assert.IsNotNull(Sut.AssembleRequestTransaction(MockOf<RequestParams>(), new RequestTransaction()));
        }

        [TestMethod]
        public void ItShouldAssembleRequestPosEvent()
        {
            Assert.IsNotNull(Sut.AssembleRequestPosEvent(new PosParams(), new RequestPosEvent()));
        }

        [TestMethod]
        public void ItShouldAssembleRequestPosEventList()
        {
            Assert.IsNotNull(Sut.AssembleRequestPosEventList(MockOf<RequestParams>(), new RequestPosEventList()));
        }

        [TestMethod]
        public void ItShouldAssembleRequestLookup()
        {
            Assert.IsNotNull(Sut.AssembleRequestLookup(MockOf<RequestParams>(), MockOf<IDevice>(), MockOf<IReceiptManager>(), LookupName.SECTORNODE));
        }

        [TestMethod]
        public void ItShouldAssembleRequestActivateForm()
        {
            Assert.IsNotNull(Sut.AssembleRequestActivate(MockOf<RequestParams>(), MockOf<IDevice>(), MockOf<IReceiptManager>(),0,"",PaymentInstant.PAYBEFORE,"","","","","",""));
        }

        [TestMethod]
        public void ItShouldAssembleRequestActivateCode()
        {
            Assert.IsNotNull(Sut.AssembleRequestActivate(MockOf<RequestParams>(), MockOf<IDevice>(), MockOf<IReceiptManager>(),""));
        }
    }
}

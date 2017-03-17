﻿// The MIT License
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
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Device;
using TruRating.TruModule.V2xx.Network;
using ISerializer = TruRating.TruModule.V2xx.Serialization.ISerializer;

namespace TruRating.TruModule.V2xx.Tests.Unit.Network
{
    [TestClass]
    public class WhenSendingAnUnsuccessfulRequest : MsTestsContext<TruServiceClient<Request, Response>>
    {
        private Request _request;
        private Response _response;
        private Response _result;
        private WebException _webException;

        [TestInitialize]
        public void Setup()
        {
            _request = new Request
            {
                PartnerId = "1",
                MerchantId = "2",
                TerminalId = "3",
            };

            RegisterFake("http://localhost");

            var webClient = MockOf<IWebClient>();
            webClient.Headers= new WebHeaderCollection();
            webClient.Stub(x=> x.ResponseHeaders).Return(new WebHeaderCollection());
            _webException = new WebException() ;
            webClient.Stub(x => x.UploadData(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<byte[]>.Is.Anything)).Throw(_webException);
            var webClientFactory = MockOf<IWebClientFactory>();
            webClientFactory.Stub(x => x.Create()).Return(webClient);
            MockOf<V2xx.Security.IMacSignatureCalculator>().Stub(x => x.EncryptionScheme).Return("3");
            MockOf<V2xx.Security.IMacSignatureCalculator>().Stub(x => x.Calculate(Arg<byte[]>.Is.Anything)).Return("secret");
            var mockOf = MockOf<ISerializer>();

            mockOf.Stub(x => x.Serialize(_request)).Return(new byte[] { 12 });
            mockOf.Stub(x => x.Deserialize<Response>(new byte[] { 12 })).Return(_response);
            _result = Sut.Send(_request);
        }

        [TestMethod]
        public void WhenCalled()
        {
           MockOf<ILogger>().AssertWasCalled(x=> x.Error(_webException, "Error in TruService Client"));
        }
    }

    [TestClass]
    public class WhenSendingASuccessfulRequest : MsTestsContext<TruServiceClient<Request,Response>>
    {
        private Request _request;
        private Response _response;
        private Response _result;
        private FakeWebClient _fakeWebClient;

        [TestInitialize]
        public void Setup()
        {
            _request = new Request()
            {
                PartnerId = "1",
                MerchantId = "2",
                TerminalId = "3",
            };

            RegisterFake("http://localhost");

            _fakeWebClient = new FakeWebClient();
            MockOf<IWebClientFactory>().Stub(x => x.Create()).Return(_fakeWebClient);
            MockOf<V2xx.Security.IMacSignatureCalculator>().Stub(x=> x.EncryptionScheme).Return("3");
            MockOf<V2xx.Security.IMacSignatureCalculator>().Stub(x=> x.Calculate(Arg<byte[]>.Is.Anything)).Return("secret");
            var mockOf = MockOf<ISerializer>();

            mockOf.Stub(x => x.Serialize(_request)).Return(new byte[] {12});
            mockOf.Stub(x => x.Deserialize<Response>(new byte[] { 12 })).Return(_response);

            _result = Sut.Send(_request);
        }

        [TestMethod]
        public void ItShouldCreateANewWebClient()
        {
            MockOf<IWebClientFactory>().AssertWasCalled(x => x.Create());
        }

        [TestMethod]
        public void ShouldReturnTheDeserializedObject()
        {
            Assert.IsTrue(_result == _response);
        }

        [TestMethod]
        public void ShouldSetTheRequiredHeaders()
        {
            Assert.IsTrue(_fakeWebClient.Headers.GetValues("x-tru-api-partner-id")[0] == "1");
            Assert.IsTrue(_fakeWebClient.Headers.GetValues("x-tru-api-merchant-id")[0] == "2");
            Assert.IsTrue(_fakeWebClient.Headers.GetValues("x-tru-api-terminal-id")[0] == "3");
            Assert.IsTrue(_fakeWebClient.Headers.GetValues("x-tru-api-encryption-scheme")[0] == "3");
            Assert.IsTrue(_fakeWebClient.Headers.GetValues("Content-Type")[0] == "application/xml; charset=utf-8");
            Assert.IsTrue(_fakeWebClient.Headers.GetValues("x-tru-api-mac")[0] == "secret");
        }
    }
}

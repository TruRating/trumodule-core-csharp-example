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
using System.IO;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.Device;
using TruRating.TruModule.Network;
using TruRating.TruModule.Security;
using TruRating.TruModule.Serialization;

namespace TruRating.TruModule.Tests.Unit.Network.TruServiceHttpClientTests
{
    [TestClass]
    public class WhenSendingAnUnsuccessfulRequest : MsTestsContext<TruServiceHttpClient>
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
            _webException = new WebException("Dummy Error", new Exception(), WebExceptionStatus.UnknownError, HttpHelpers.CreateWebResponse(HttpStatusCode.InternalServerError, new MemoryStream(Encoding.UTF8.GetBytes("Server Error!"))));
            webClient.Stub(x => x.UploadData(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<byte[]>.Is.Anything)).Throw(_webException);
            var webClientFactory = MockOf<IWebClientFactory>();
            webClientFactory.Stub(x => x.Create()).Return(webClient);
            MockOf<IMacSignatureCalculator>().Stub(x => x.EncryptionScheme).Return("3");
            MockOf<IMacSignatureCalculator>().Stub(x => x.Calculate(Arg<byte[]>.Is.Anything)).Return("secret");
            var mockOf = MockOf<ISerializer>();

            mockOf.Stub(x => x.Serialize(_request)).Return(new byte[] { 12 });
            mockOf.Stub(x => x.Deserialize<Response>(new byte[] { 12 })).Return(_response);
            _result = Sut.Send(_request);
        }

        [TestMethod]
        public void ItShouldLogTheWebException()
        {
            MockOf<ILogger>().AssertWasCalled(x=> x.Error(_webException, "Error in TruService Client"));
        }

        [TestMethod]
        public void ItShouldLogTheServerResponseBody()
        {
            MockOf<ILogger>().AssertWasCalled(x => x.Error("Server Error!"));
        }
    }
}
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
using TruRating.Dto.TruService.V220;
using TruRating.Dto.TruService.V2xx;

namespace TruRating.TruModule.V2xx.Environment
{
    public interface IHttpClient
    {
        Response Send(Request request);
    }

    public class HttpClient : IHttpClient
    {
        private static readonly WebClient WebClient = new WebClient {Encoding = Encoding.UTF8};
        private readonly ILogger _logger;
        private readonly IMacSignatureCalculator _macSignatureCalculator;
        private readonly ISettings _settings;

        public HttpClient(ISettings settings, ILogger logger, IMacSignatureCalculator macSignatureCalculator)
        {
            _settings = settings;
            _logger = logger;
            _macSignatureCalculator = macSignatureCalculator;
        }

        public Response Send(Request request)
        {
            string targetNamespace;
            switch (_settings.TsiVersion)
            {
                case TsiVersion.V200:
                    targetNamespace = Validation.TruServiceV200.TargetNamespace;
                    break;
                case TsiVersion.V210:
                    targetNamespace = Validation.TruServiceV210.TargetNamespace;
                    break;
                case TsiVersion.V220:
                    targetNamespace = Validation.TruServiceV220.TargetNamespace;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            WebClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");

            WebClient.Headers.Set("x-tru-api-partner-id", request.PartnerId);
            WebClient.Headers.Set("x-tru-api-merchant-id", request.MerchantId);
            WebClient.Headers.Set("x-tru-api-terminal-id", request.TerminalId);
            WebClient.Headers.Set("x-tru-api-encryption-scheme", "3");

            var serializeString = Serializer.SerializeString(request, targetNamespace, new UTF8Encoding());
            if (!string.IsNullOrEmpty(_settings.TransportKey))
            {
                try
                {
                    WebClient.Headers.Set("x-tru-api-mac",
                        _macSignatureCalculator.Calculate(serializeString, _settings.TransportKey));
                }
                catch (Exception)
                {
                    _logger.WriteLine(ConsoleColor.Red, "HttpClient : Invalid TransportKey");
                }
            }
            _logger.WriteDebug(string.Format("POST {0} HTTP/1.1", _settings.Endpoint), ConsoleColor.Yellow);
            foreach (var header in WebClient.Headers)
            {
                _logger.WriteDebug(string.Format("{0} : {1}", header, WebClient.Headers[header.ToString()]),
                    ConsoleColor.Yellow);
            }
            _logger.WriteDebug("", ConsoleColor.Yellow);
            _logger.WriteDebug(serializeString, ConsoleColor.Yellow);
            string responseBody;
            try
            {
                responseBody = WebClient.UploadString(_settings.Endpoint, "POST", serializeString);
            }
            catch (WebException e)
            {
                using (var reader = new StreamReader(e.Response.GetResponseStream()))
                {
                    var result = reader.ReadToEnd(); // do something fun...
                    _logger.WriteDebug(result, ConsoleColor.Red);
                }
                throw;
            }
            if (WebClient.ResponseHeaders.Get("x-tru-api-diagnostic") != null)
            {
                _logger.WriteLine(ConsoleColor.Red, "HttpClient : {0}", WebClient.ResponseHeaders.Get("x-tru-api-diagnostic"));
            }
            _logger.WriteDebug(responseBody);
            _logger.WriteDebug("");
            return Serializer.Deserialize<Response>(responseBody);
        }
    }
}
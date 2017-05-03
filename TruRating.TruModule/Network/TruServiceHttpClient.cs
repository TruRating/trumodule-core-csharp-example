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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.Device;
using TruRating.TruModule.Security;
using TruRating.TruModule.Serialization;
using TruRating.TruModule.Settings;

namespace TruRating.TruModule.Network
{
    public class TruServiceHttpClient : ITruServiceClient
    {
        private readonly string _endpoint;
        private readonly ILogger _logger;
        private readonly IMacSignatureCalculator _macSignatureCalculator;

        public TruServiceHttpClient(string endpoint, IMacSignatureCalculator macSignatureCalculator, ISerializer serializer,ILogger logger, IWebClientFactory webClientFactory)
        {
            _endpoint = endpoint;
            _macSignatureCalculator = macSignatureCalculator;
            Serializer = serializer;
            _logger = logger;
            WebClientFactory = webClientFactory;
        }

        internal ISerializer Serializer { get; private set; }

        internal IWebClientFactory WebClientFactory { get; private set; }

        public static ITruServiceClient CreateDefault(string endpoint, ILogger logger, IMacSignatureCalculator macSignatureCalculator, ISerializer serializer, IWebClientFactory webClientFactory)
        {
            return new TruServiceHttpClient(endpoint,macSignatureCalculator,serializer,logger, webClientFactory);
        }
        internal static ITruServiceClient CreateDefault(ILogger logger, ISettings settings)
        {
            return CreateDefault(settings.TruServiceUrl, logger, new MacSignatureCalculator(settings.TransportKey, logger), new DefaultSerializer(), new SystemWebClientFactory(settings.HttpTimeoutMs));
        }

        public Response Send(Request request)
        {
            var deserialize = default(Response);

            try
            {
                var webClient = WebClientFactory.Create();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");
                webClient.Headers.Set("x-tru-api-partner-id", request.PartnerId);
                webClient.Headers.Set("x-tru-api-merchant-id", request.MerchantId);
                webClient.Headers.Set("x-tru-api-terminal-id", request.TerminalId);
                webClient.Headers.Set("x-tru-api-encryption-scheme", _macSignatureCalculator.EncryptionScheme);
                var requestBody = Serializer.Serialize(request);
                webClient.Headers.Set("x-tru-api-mac", _macSignatureCalculator.Calculate(requestBody));
                Trace.TraceInformation("POST {0} HTTP/1.1", _endpoint);
                foreach (var header in webClient.Headers)
                {
                    _logger.Debug("{0} : {1}", header, webClient.Headers[header.ToString()]);
                }
                _logger.Debug("Request Body\n{0}", Encoding.UTF8.GetString(requestBody));
                byte[] responseBody;
                try
                {
                    responseBody = webClient.UploadData(_endpoint, "POST", requestBody);
                }
                catch (WebException e)
                {
                    if (e.Response != null)
                        using (var reader = new StreamReader(e.Response.GetResponseStream()))
                        {
                            var result = reader.ReadToEnd();
                            _logger.Error(result);
                        }
                    throw;
                }
                if (webClient.ResponseHeaders != null && webClient.ResponseHeaders.Get("x-tru-api-diagnostic") != null)
                {
                    _logger.Warn("{0}",webClient.ResponseHeaders.Get("x-tru-api-diagnostic"));
                }
                _logger.Debug("Response Body\n{0}", Encoding.UTF8.GetString(responseBody));
                deserialize = Serializer.Deserialize<Response>(responseBody);
                return deserialize;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error in TruService Client");
            }
            return deserialize;
        }
    }
}
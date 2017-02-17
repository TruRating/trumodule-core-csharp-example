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
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Environment;

namespace TruRating.TruModule.V2xx.Messages
{
    public class TsiV220Messages : TsiV210Messages, ITsiV220Messages
    {
        public TsiV220Messages(ISettings settings, ILogger logger, IHttpClient httpClient)
            : base(settings, logger, httpClient)
        {
        }

        public Response SendRequestQuery()
        {
            var request = CreateBlankRequest(Guid.NewGuid().ToString());

            request.Item = new RequestQuery
            {
                Force = true,
                ForceSpecified = true,
                Language = CreateRequestLanguage(),
                Device = CreateRequestDevice(),
                Server = CreateRequestServer()
            };
            return HttpClient.Send(request);
        }

        public Response SendRequestLookup()
        {
            var request = CreateBlankRequest(Guid.NewGuid().ToString());

            request.Item = new RequestLookup
            {
                Name = LookupName.SECTORNODE,
                Language = CreateRequestLanguage(),
                Device = CreateRequestDevice(),
                Server = CreateRequestServer()
            };
            return HttpClient.Send(request);
        }

        public Response SendRequestActivate()
        {
            var request = CreateBlankRequest(Guid.NewGuid().ToString());

            var requestActivate = new RequestActivate
            {
                Language = CreateRequestLanguage(),
                Device = CreateRequestDevice(),
                Server = CreateRequestServer()
            };
            if (Settings.RegistrationCode)
            {
                requestActivate.Item = System.Environment.MachineName;
            }
            else
            {
                requestActivate.Item = new RequestRegistrationForm
                {
                    BusinessAddress = "Bankside House, 107 Leadenhall St, London EC3A 4AF",
                    BusinessName =
                        string.Format("{0} @ {1}", System.Environment.MachineName, ExtensionMethods.GetLocalIpAddress()),
                    MerchantEmailAddress = "developers@trurating.com",
                    MerchantMobileNumber = "0330 123 1310",
                    MerchantName = "TruRating Developers",
                    MerchantPassword = "password",
                    PaymentInstant = PaymentInstant.PAYBEFORE,
                    SectorNode = 137,
                    TimeZone = "Europe/London"
                };
            }
            request.Item = requestActivate;

            return HttpClient.Send(request);
        }
    }
}
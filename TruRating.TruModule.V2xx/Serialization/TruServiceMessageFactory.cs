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

using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Device;
using TruRating.TruModule.V2xx.Module;

namespace TruRating.TruModule.V2xx.Serialization
{
    public class TruServiceMessageFactory : ITruServiceMessageFactory
    {
        public Request AssemblyQueryRequest(IDevice device,
            string partnerId,
            string merchantId,
            string terminalId,
            string sessionId,
            bool force)
        {
            var result = new Request
            {
                PartnerId = partnerId,
                MerchantId = merchantId,
                TerminalId = terminalId,
                SessionId = sessionId,
                Item = new RequestQuery
                {
                    Device = device.GetRequestDevice(),
                    Language = device.GetLanguages(),
                    Server = device.GetServer(),
                    ForceSpecified = true,
                    Force = force
                }
            };
            return result;
        }

        public Request AssembleQuestionRequest(
            IDevice device,
            string partnerId,
            string merchantId,
            string terminalId,
            string sessionId,
            Trigger trigger)
        {
            var result = new Request
            {
                PartnerId = partnerId,
                MerchantId = merchantId,
                TerminalId = terminalId,
                SessionId = sessionId,
                Item = new RequestQuestion
                {
                    Trigger = trigger,
                    Device = device.GetRequestDevice(),
                    Language = device.GetLanguages(),
                    Server = device.GetServer()
                }
            };
            return result;
        }

        public Request AssembleTransactionRequest(string partnerId, string sessionId, string merchantId,
            string terminalId, RequestTransaction requestTransaction)
        {
            var result = new Request
            {
                PartnerId = partnerId,
                MerchantId = merchantId,
                TerminalId = terminalId,
                SessionId = sessionId,
                Item = requestTransaction
            };
            return result;
        }

        public Request AssemblePosEventRequest(PosParams posParams, RequestPosEvent requestPosEvent)
        {
            var result = new Request
            {
                PartnerId = posParams.PartnerId,
                MerchantId = posParams.MerchantId,
                TerminalId = posParams.TerminalId,
                SessionId = posParams.SessionId,
                Item = requestPosEvent
            };
            return result;
        }

        public Request AssemblePosEventRequest(PosParams posParams, RequestPosEventList requestPosEventList)
        {
            var result = new Request
            {
                PartnerId = posParams.PartnerId,
                MerchantId = posParams.MerchantId,
                TerminalId = posParams.TerminalId,
                SessionId = posParams.SessionId,
                Item = requestPosEventList
            };
            return result;
        }

        public Request AssembleRequestLookup(IDevice device, string partnerId, string merchantId, string terminalId,
            string sessionId, LookupName lookupName)
        {
            var result = new Request
            {
                PartnerId = partnerId,
                MerchantId = merchantId,
                TerminalId = terminalId,
                SessionId = sessionId,
                Item = new RequestLookup
                {
                    Name = lookupName,
                    Device = device.GetRequestDevice(),
                    Language = device.GetLanguages(),
                    Server = device.GetServer()
                }
            };
            return result;
        }

        public Request AssembleRequestActivate(IDevice device, string partnerId, string merchantId, string terminalId,
            string sessionId, int sectorNode, string timeZone, PaymentInstant paymentInstant, string emailAddress,
            string password, string address, string mobileNumber, string merchantName, string businessName)
        {
            var result = new Request
            {
                PartnerId = partnerId,
                MerchantId = merchantId,
                TerminalId = terminalId,
                SessionId = sessionId,
                Item = new RequestActivate
                {
                    Device = device.GetRequestDevice(),
                    Language = device.GetLanguages(),
                    Server = device.GetServer(),
                    Item = new RequestRegistrationForm
                    {
                        BusinessAddress = address,
                        BusinessName = businessName,
                        MerchantEmailAddress = emailAddress,
                        MerchantMobileNumber = mobileNumber,
                        MerchantName = merchantName,
                        MerchantPassword = password,
                        PaymentInstant = paymentInstant,
                        SectorNode = sectorNode,
                        TimeZone = timeZone
                    }
                }
            };
            return result;
        }

        public Request AssembleRequestActivate(IDevice device, string partnerId, string merchantId, string terminalId,
            string sessionId, string registrationCode)
        {
            var result = new Request
            {
                PartnerId = partnerId,
                MerchantId = merchantId,
                TerminalId = terminalId,
                SessionId = sessionId,
                Item = new RequestActivate
                {
                    Device = device.GetRequestDevice(),
                    Language = device.GetLanguages(),
                    Server = device.GetServer(),
                    Item = registrationCode
                }
            };
            return result;
        }
    }
}
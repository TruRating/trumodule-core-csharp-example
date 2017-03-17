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
using TruRating.Dto.TruService.V2xx;
using TruRating.TruModule.V2xx.Device;
using TruRating.TruModule.V2xx.Module;

namespace TruRating.TruModule.V2xx.Serialization
{
    public interface ITruServiceMessageFactory
    {
        Request AssemblyRequestQuery(IDevice device,
            string partnerId,
            string merchantId,
            string terminalId,
            string sessionId,
            bool force);

        Request AssembleRequestQuestion(IDevice device,
            string partnerId,
            string merchantId,
            string terminalId,
            string sessionId,
            Trigger trigger);

        Request AssembleRequestTransaction(string partnerId, string sessionId, string merchantId, string terminalId,
            RequestTransaction requestTransaction);

        Request AssembleRequestPosEvent(PosParams posParams, RequestPosEvent requestPosEvent);
        Request AssembleRequestPosEvent(PosParams posParams, RequestPosEventList requestPosEventList);

        Request AssembleRequestLookup(IDevice device, string partnerId, string merchantId, string terminalId,
            string sessionId, LookupName lookupName);

        Request AssembleRequestActivate(IDevice device, string partnerId, string merchantId, string terminalId,
            string sessionId, int sectorNode, string timeZone, PaymentInstant paybefore, string emailAddress,
            string password, string address, string mobileNumber, string merchantName, string businessName);

        Request AssembleRequestActivate(IDevice device, string partnerId, string merchantId, string terminalId,
            string sessionId, string registrationCode);

        Request AssembleRatingRequest(IServiceMessage serviceMessage, RequestRating rating);
    }
}
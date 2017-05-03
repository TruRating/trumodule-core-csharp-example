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
using TruRating.TruModule.Device;

namespace TruRating.TruModule.Messages
{
    public interface ITruServiceMessageFactory
    {
        Request AssemblyRequestQuery(RequestParams requestParams, IDevice device, IReceiptManager receiptManager, bool force);
        Request AssembleRequestQuestion(RequestParams requestParams, IDevice device, IReceiptManager receiptManager, Trigger trigger);
        Request AssembleRequestTransaction(RequestParams requestParams,  RequestTransaction requestTransaction);
        Request AssembleRequestPosEvent(PosParams posParams, RequestPosEvent requestPosEvent);
        Request AssembleRequestPosEventList(RequestParams requestParams,  RequestPosEventList requestPosEventList);
        Request AssembleRequestLookup(RequestParams requestParams, IDevice device, IReceiptManager receiptManager, LookupName lookupName);
        Request AssembleRequestActivate(RequestParams requestParams, IDevice device, IReceiptManager receiptManager, int sectorNode, string timeZone, PaymentInstant paybefore, string emailAddress, string password, string address, string mobileNumber, string merchantName, string businessName);
        Request AssembleRequestActivate(RequestParams requestParams, IDevice device, IReceiptManager receiptManager, string registrationCode);
        Request AssembleRequestRating(Request request, RequestRating rating);
    }
}
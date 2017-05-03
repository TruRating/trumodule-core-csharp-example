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
using TruRating.TruModule.Settings;

namespace TruRating.TruModule.Messages
{
    public class RequestParams
    {
        public RequestParams()
        {
            
        }
        public RequestParams(PosParams posParams)
        {
            PartnerId = posParams.PartnerId;
            MerchantId = posParams.MerchantId;
            TerminalId = posParams.TerminalId;
            SessionId = posParams.SessionId;
            Url = posParams.Url;
        }
        public RequestParams(ISettings settings, string sessionId)
        {
            PartnerId = settings.PartnerId;
            MerchantId = settings.MerchantId;
            TerminalId = settings.TerminalId;
            SessionId = sessionId;
            Url = settings.TruServiceUrl;
        }
        public string PartnerId { get; set; }
        public string MerchantId { get; set; }
        public string TerminalId { get; set; }
        public string SessionId { get; set; }
        public string Url { get; set; }
    }
    public class PosParams
    {
        public string PartnerId { get; set; }
        public string MerchantId { get; set; }
        public string TerminalId { get; set; }
        public string SessionId { get; set; }
        public string Url { get; set; }
    }
}
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

namespace TruRating.TruModule.Settings
{
    /// <summary>
    /// Contains the initial setting values required by TruModule to communicate with TruService
    /// </summary>
    public interface ISettings
    {
        /// <summary>
        /// Identifies the Partner, specified by TruRating
        /// </summary>
        string PartnerId { get; set; }
        /// <summary>
        /// Identifies the physical merchant outlet, known to both the Partner and TruRating, specified by the Partner
        /// </summary>
        string MerchantId { get; set; }
        /// <summary>
        /// Identifies the payment device, specified by the Partner
        /// </summary>
        string TerminalId { get; set; }
        /// <summary>
        /// Specifies the URL of TruService, in RFCXXXX format {protocol}://{uri}:{port}/api/servicemessage
        /// </summary>
        string TruServiceUrl { get; set; }
        /// <summary>
        /// Specifies the TransportKey used to generate a MAC header for authentication by TruService
        /// </summary>
        string TransportKey { get; set; }
        /// <summary>
        /// Specifies the timeout in milliseconds to wait for a response from TruService
        /// </summary>
        int HttpTimeoutMs { get; }
        /// <summary>
        /// Specifies the Trigger point for the TruRating question.  May be reconfigured dynamically by TruService.
        /// </summary>
        Trigger Trigger { get; set; }
    }
}
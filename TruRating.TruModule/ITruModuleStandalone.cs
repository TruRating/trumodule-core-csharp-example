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

namespace TruRating.TruModule
{
    public interface ITruModuleStandalone
    {
        /// <summary>
        /// Called at the defined trigger point to capture a rating.  A call to SendTransaction is expected after this call.
        /// </summary>
        void DoRating();
        /// <summary>
        /// Called after a call to DoRating when the transaction result has been returned by the payment application
        /// </summary>
        /// <param name="requestTransaction"></param>
        void SendTransaction(RequestTransaction requestTransaction);
        /// <summary>
        /// Called if the rating needs to be cancelled.  A call to SendTransaction is expected after this call.
        /// </summary>
        void CancelRating();
        /// <summary>
        /// Activate TruRating on this device using details captured via the UI
        /// </summary>
        /// <param name="sectorNode">Defines the business sector for this outlet, accepted values can be retrieved by a call to GetLookups(LookupName.SectorNode)</param>
        /// <param name="timeZone">Defines the timezone for this outlet, accepted values can be retrieved by a call to GetLookups(LookupName.TimeZone)</param>
        /// <param name="paymentInstant">Defines the payment instance for this outlet</param>
        /// <param name="emailAddress">The merchant's email address</param>
        /// <param name="password">The merchant's requested password</param>
        /// <param name="address">The physical outlet address</param>
        /// <param name="mobileNumber">The merchant's mobile number</param>
        /// <param name="merchantName">The merchant's name</param>
        /// <param name="businessName">The Business name</param>
        /// <returns></returns>
        bool Activate(int sectorNode, string timeZone, PaymentInstant paymentInstant, string emailAddress, string password, string address,string mobileNumber, string merchantName, string businessName);
        /// <summary>
        /// Activte TruRating on this device using a registration code generated at trurating.com
        /// </summary>
        /// <param name="registrationCode"></param>
        /// <returns></returns>
        bool Activate(string registrationCode);
        /// <summary>
        /// Check if this instance of TruModule is activated or not
        /// </summary>
        /// <param name="bypassTruServiceCache">false if using cached values on TruService, true if requesting the most up-to-date activation status</param>
        /// <returns></returns>
        bool IsActivated(bool bypassTruServiceCache);
        /// <summary>
        /// Retrieve a set of lookup option names and values for a given lookupName. Results can be presented to the end user and values are used as specified in calls to Activate()
        /// </summary>
        /// <param name="lookupName">Specifies the lookup</param>
        /// <returns></returns>
        LookupOption[] GetLookups(LookupName lookupName);
    }
}
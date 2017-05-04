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
using TruRating.TruModule.Util;

namespace TruRating.TruModule.Device
{
    /// <summary>
    /// Represents a logical PinPad that can display text and capture basic key input
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// Displays an arbitrary message on screen that can be removed at any time and can be removed at any time
        /// </summary>
        /// <param name="value">The text to display</param>
        void DisplayMessage(string value);

        /// <summary>
        /// Displays an arbitrary acknowledgement on screen that should be removed after the specified timeout
        /// </summary>
        /// <param name="value">The text to display</param>
        /// <param name="timeoutMilliseconds">The length of time the text should appear</param>
        /// <param name="hasRated">Indicates whether the customer rated</param>
        /// <param name="ratingContext">Indicates the context of the acknowledgement</param>
        void DisplayAcknowledgement(string value, int timeoutMilliseconds, bool hasRated, RatingContext ratingContext);
        /// <summary>
        /// Displays an arbitrary question on screen that should capture a key press and should be removed after the specified timeout
        /// Valid pinpad values are 0, 1, 2, 3, 4, 5, 6, 7, 8, 9
        /// -1 should be used for any other key input
        /// -2 should be used if the timeout is reached
        /// -4 should be used if the question cannot be displayed
        /// </summary>
        /// <param name="value">The text to display</param>
        /// <param name="timeoutMilliseconds">The length of time the text should appear</param>
        /// <returns>Valid pinpad values are 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, -1 should be used for any other key input,  -2 should be used if the timeout is reached, -4 should be used if the question cannot be displayed</returns>
        short Display1AQ1KR(string value, int timeoutMilliseconds);
        /// <summary>
        /// Remove any message on screen and cancel any 1AQ1KR routine and reset to default state.
        /// </summary>
        void ResetDisplay();
        /// <summary>
        /// Returns the capabilites of the Screen
        /// </summary>
        /// <returns>RequestPeripheral</returns>
        RequestPeripheral GetScreenCapabilities();
        /// <summary>
        /// Returns any hardcoded skip instruction on the display
        /// </summary>
        /// <returns></returns>
        SkipInstruction GetSkipInstruction();
        /// <summary>
        /// Gets the manufacturer's device model name, e.g. IPP350
        /// </summary>
        /// <returns></returns>
        string GetName();
        /// <summary>
        /// Gets the manufacturer's firmware version, e.g. RAM0973
        /// </summary>
        /// <returns></returns>
        string GetFirmware();
        /// <summary>
        /// Get all languages that can be used by the pinpad
        /// </summary>
        /// <returns></returns>
        RequestLanguage[] GetLanguages();
        /// <summary>
        /// Gets the current language of the pinpad
        /// </summary>
        /// <returns></returns>
        string GetCurrentLanguage();
        /// <summary>
        /// Gets any server that is proxying requests on behalf of the pinpad.
        /// </summary>
        /// <returns></returns>
        RequestServer GetServer();
    }
}
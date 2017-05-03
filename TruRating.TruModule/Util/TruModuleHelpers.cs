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

namespace TruRating.TruModule.Util
{
    class TruModuleHelpers
    {
        internal static bool QuestionAvailable(Response response, string language, out ResponseQuestion responseQuestion,
            out ResponseReceipt[] responseReceipts, out ResponseScreen[] responseScreens)
        {
            responseQuestion = null;
            responseReceipts = null;
            responseScreens = null;
            if (response == null) //TODO check for null?
            {
                throw new ArgumentNullException(nameof(response));
            }
            if (language == null)
            {
                throw new ArgumentNullException(nameof(language));
            }
            var item = response.Item as ResponseDisplay;
            if (item != null)
            {
                var responseDisplay = item;
                if (responseDisplay.Language != null)
                    foreach (var responseLanguage in responseDisplay.Language)
                    {
                        if (language.Equals(responseLanguage.Rfc1766) && responseLanguage.Question != null)
                        {
                            responseQuestion = responseLanguage.Question;
                            responseReceipts = responseLanguage.Receipt;
                            responseScreens = responseLanguage.Screen;
                            return true;
                        }
                    }
                return false;
            }

            return false;
        }

        internal static string GetResponseReceipt(ResponseReceipt[] responseReceipts, When when)
        {
            if (responseReceipts == null)
                return null;
            foreach (var responseReceipt in responseReceipts)
            {
                if (responseReceipt.When == when)
                {
                    return responseReceipt.Value;
                }
            }
            return null;
        }

        internal static ResponseScreen GetResponseScreen(ResponseScreen[] responseScreens, When whenToDisplay)
        {
            if (responseScreens == null)
                return null;
            foreach (var responseScreen in responseScreens)
            {
                if (responseScreen.When == whenToDisplay) //If this response element matches the state of the screen.
                {
                    return responseScreen;
                }
            }
            return null;
        }
    }
}
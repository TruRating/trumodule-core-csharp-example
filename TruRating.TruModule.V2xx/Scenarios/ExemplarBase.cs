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
using System.Globalization;
using System.Threading;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Environment;

namespace TruRating.TruModule.V2xx.Scenarios
{
    internal interface IExemplar
    {
        void Scenario();
        void RunLoop();
        bool IsApplicable();
    }

    public abstract class ExemplarBase : IExemplar
    {
        protected static readonly Random Rand = new Random();
        protected readonly IDevice Device;
        protected readonly ILogger Logger;
        protected readonly ISettings Settings;

        protected ExemplarBase(ILogger logger, ISettings settings, IDevice device)
        {
            Logger = logger;
            Settings = settings;
            Device = device;
        }

        public void RunLoop()
        {
            Logger.WriteLine(ConsoleColor.Red, "Press any key to start");
            KeyPressReader.ReadKey();
            while (true) //Endless loop
            {
                try
                {
                    Scenario();
                }
                catch (Exception e)
                {
                    Logger.WriteLine(ConsoleColor.DarkYellow, "Error {0}", e);
                }
                finally
                {
                    var endOfRunPressAKeyToReset = " End of run. Press a key to reset. ";
                    Logger.WriteLine(ConsoleColor.DarkGray,
                        endOfRunPressAKeyToReset.PadLeft(Console.WindowWidth - 1, '='));
                    Logger.WriteLine(ConsoleColor.DarkGray, "");
                    KeyPressReader.ReadKey();
                }
            }
        }

        public abstract void Scenario();

        public abstract bool IsApplicable();

        protected RequestRating CaptureRating(Response response, string language)
        {
            ResponseQuestion question;
            ResponseReceipt[] receipts;
            ResponseScreen[] screens;
            RequestRating rating = null;

            if (PrepareQuestion(response, language, out question, out receipts, out screens))
                //Look through the response for a valid question
            {
                rating = new RequestRating //Have a question, construct the basic rating record
                {
                    DateTime = DateTime.UtcNow,
                    Rfc1766 = language,
                    Operator = "Toby"
                };
                if (!HasQuestionInCurrentLanguage(question, language))
                {
                    //Cant display the question due to a local issue set the rating value to -4 OR
                    //Cant display the question in the requested language(s)
                    rating.Value = -4;
                }
                else
                {
                    Settings.LastQuestionDateTime = DateTime.UtcNow;
                    try
                    {
                        var sw = new Stopwatch();
                        sw.Start();
                        var keypress = Device._1AQ1KR(question.Value, question.TimeoutMs);
                        //Wait for the user input for the specified period
                        sw.Stop();
                        rating.ResponseTimeMs = (int) sw.ElapsedMilliseconds; //Set the response time
                        short result;
                        if(short.TryParse(keypress.ToString(), out result))
                        {
                            rating.Value = result;
                        }
                        else
                        {
                            rating.Value = -1; //User didn't press a number
                        }

                        if (rating.Value == -1)
                        {
                            //Show the not rated text
                            WriteResponseReceipt(receipts, When.NOTRATED);
                            WaitResponseScreen(screens, When.NOTRATED);
                        }
                        else
                        {
                            //Show the rated text
                            WriteResponseReceipt(receipts, When.RATED);
                            WaitResponseScreen(screens, When.RATED);
                        }
                    }
                    catch (TimeoutException) //Timer expired
                    {
                        //Show the not rated text
                        rating.Value = -2;
                        WriteResponseReceipt(receipts, When.NOTRATED);
                        WaitResponseScreen(screens, When.NOTRATED);
                    }
                }
            }
            return rating;
        }

        private bool HasQuestionInCurrentLanguage(ResponseQuestion responseQuestion, string language)
        {
            if (responseQuestion == null)
            {
                return false;
            }
            return true;
        }

        private static bool PrepareQuestion(Response response, string language, out ResponseQuestion responseQuestion,
            out ResponseReceipt[] responseReceipts, out ResponseScreen[] responseScreens)
        {
            responseQuestion = null;
            responseReceipts = null;
            responseScreens = null;
            var item = response.Item as ResponseDisplay;
            if (item != null)
            {
                var responseDisplay = item;
                foreach (var responseLanguage in responseDisplay.Language)
                {
                    if (responseLanguage.Rfc1766 == language)
                    {
                        responseQuestion = responseLanguage.Question;
                        responseReceipts = responseLanguage.Receipt;
                        responseScreens = responseLanguage.Screen;
                        return true;
                    }
                }
                return true;
            }

            return false;
        }

        private void WriteResponseReceipt(ResponseReceipt[] responseReceipts, When when)
        {
            if (responseReceipts == null)
                return;
            foreach (var responseReceipt in responseReceipts)
            {
                if (responseReceipt.When == when)
                {
                    Device.PrintReceipt(responseReceipt.Value);
                    break;
                }
            }
        }

        private void WaitResponseScreen(ResponseScreen[] responseScreens, When whenToDisplay)
        {
            if (responseScreens == null)
                return;
            foreach (var responseScreen in responseScreens)
            {
                if (responseScreen.When == whenToDisplay) //If this response element matches the state of the screen.
                {
                    try
                    {
                        Device._1AQ1KR(responseScreen.Value, responseScreen.TimeoutMs);
                    }
                    catch (TimeoutException)
                    {
                    }
                    break;
                }
            }
        }
    }
}
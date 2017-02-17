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
            Logger.Write(ConsoleColor.Green, "Press any key to start");
            KeyPressReader.ReadKey();
            while (true) //Endless loop
            {
                try
                {
                    Scenario();
                }
                catch (Exception e)
                {
                    Logger.Write(ConsoleColor.DarkYellow, "Error {0}", e);
                }
                finally
                {
                    if (!Settings.Automatic)
                    {
                        var endOfRunPressAKeyToReset = " End of run. Press a key to reset. ";
                        Logger.Write(ConsoleColor.DarkGray,
                            endOfRunPressAKeyToReset.PadLeft(Console.WindowWidth - 1, '='));
                        Logger.Write(ConsoleColor.DarkGray, "");
                        KeyPressReader.ReadKey();
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
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
                    Device.PrintScreen(question.Value); //Show the question
                    try
                    {
                        var sw = new Stopwatch();
                        sw.Start();
                        Device.Log("waiting {0} ms", question.TimeoutMs);

                        if (Settings.Automatic)
                        {
                            var rand = new Random();
                            Thread.Sleep(rand.Next(500, 2000));
                            rating.Value = (short) rand.Next(-1, 9);
                        }
                        else
                        {
                            var keypress = Device.ReadKey(question.TimeoutMs);
                            //Wait for the user input for the specified period
                            sw.Stop();
                            rating.ResponseTimeMs = (int) sw.ElapsedMilliseconds; //Set the response time

                            if (IsZeroToNineKeyPressed(keypress.Key))
                            {
                                rating.Value = short.Parse(keypress.KeyChar.ToString());
                            }
                            else
                            {
                                rating.Value = -1; //User didn't press a number
                            }
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

        private bool IsZeroToNineKeyPressed(ConsoleKey keypress)
        {
            if (keypress >= ConsoleKey.D0 && keypress <= ConsoleKey.D9)
            {
                return true;
            }

            if (keypress >= ConsoleKey.NumPad0 && keypress <= ConsoleKey.NumPad9)
            {
                return true;
            }

            return false;
        }

        private bool HasQuestionInCurrentLanguage(ResponseQuestion responseQuestion, string language)
        {
            if (responseQuestion == null)
            {
                Device.Error("Can't ask a question because transaction conducted in {0}", language);
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
                    Device.PrintScreen(responseScreen.Value);
                    try
                    {
                        if (!Settings.Automatic)
                        {
                            Device.Log("waiting {0} ms", responseScreen.TimeoutMs);
                            Device.ReadKey(responseScreen.TimeoutMs);
                        }
                        else
                        {
                            Thread.Sleep(500);
                        }
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
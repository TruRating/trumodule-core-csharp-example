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
using System.Collections.Generic;
using System.Reflection;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.ConsoleRunner.Environment;
using TruRating.TruModule.Device;

namespace TruRating.TruModule.ConsoleRunner.Device
{
    public class ConsoleDevice : IDevice
    {
        private readonly string[] _languages;
        private readonly IConsoleLogger _consoleLogger;

        public ConsoleDevice(IConsoleLogger consoleLogger, string[] languages)
        {
            _consoleLogger = consoleLogger;
            _languages = languages;
        }

        public void DisplayMessage(string value)
        {
            _consoleLogger.WriteLine(ConsoleColor.White, "DISPLAY: " + value);
        }
        public RequestPeripheral GetScreenCapabilities()
        {
            return new RequestPeripheral
            {
                Format = Format.TEXT,
                Separator = System.Environment.NewLine,
                Font = Font.MONOSPACED,
                Height = 4,
                HeightSpecified = true,
                Unit = UnitDimension.LINE,
                Width = 16,
                WidthSpecified = true
            };
        }

        public SkipInstruction GetSkipInstruction()
        {
            return SkipInstruction.NONE;
        }

        public string GetName()
        {
            return GetType().Name;
        }

        public string GetFirmware()
        {
            return Assembly.GetExecutingAssembly().FullName;
        }

        public void CancelQuestion()
        {
            KeyPressReader.Cancel();
        }

        public RequestLanguage[] GetLanguages()
        {
            var result = new List<RequestLanguage>();
            foreach (var language in _languages)
            {
                result.Add(new RequestLanguage {Rfc1766 = language});
            }
            return result.ToArray();
        }

        public string GetCurrentLanguage()
        {
            return GetLanguages()[0].Rfc1766;
        }

        public RequestServer GetServer()
        {
            return null;
        }
        public void DisplayAcknowledgement(string value, int timeoutMilliseconds, bool hasRated, RatingContext ratingContext)
        {
            _consoleLogger.WriteLine(ConsoleColor.White, "DISPLAY: " + value);
            _consoleLogger.WriteLine(ConsoleColor.Gray, "DISPLAY: waiting {0} ms", timeoutMilliseconds);
            try
            {
                KeyPressReader.ReadKey(timeoutMilliseconds, true);
            }
            catch (TimeoutException)
            {
                //Suppress
            }
        }

        public short Display1AQ1KR(string value, int timeoutMilliseconds)
        {
            try
            {
                _consoleLogger.WriteLine(ConsoleColor.Cyan, "1AQ1KR : " + value);
                _consoleLogger.WriteLine(ConsoleColor.Gray, "1AQ1KR : waiting {0} ms", timeoutMilliseconds);
                _consoleLogger.Write(ConsoleColor.Cyan, "1AQ1KR : ");
                short result;
                if (short.TryParse(KeyPressReader.ReadKey(timeoutMilliseconds, false).KeyChar.ToString(), out result))
                {
                    return result;
                }
                return -1; //User didn't press a number
            }
            catch (TimeoutException)
            {
                return -2; //User timed out
            }
            catch (Exception)
            {
                return -4; // Couldn't ask a question or capture the response
            }
        }
     
    }
}
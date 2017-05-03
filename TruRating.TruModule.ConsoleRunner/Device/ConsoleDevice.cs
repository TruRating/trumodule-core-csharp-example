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
    public class ConsoleDevice : IDevice, IReceiptManager
    {
        private readonly IConsoleSettings _consoleSettings;
        private readonly IConsoleIo _logger;

        public ConsoleDevice(IConsoleIo logger, IConsoleSettings consoleSettings)
        {
            _logger = logger;
            _consoleSettings = consoleSettings;
        }

        public void DisplayMessage(string value)
        {
            _logger.WriteLine(ConsoleColor.White, "DISPLAY: " + value);
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

        public RequestPeripheral GetReceiptCapabilities()
        {
            return new RequestPeripheral
            {
                Format = Format.TEXT,
                Separator = System.Environment.NewLine,
                Font = Font.MONOSPACED,
                Unit = UnitDimension.LINE,
                Width = 40,
                WidthSpecified = true
            };
        }

        public void ResetDisplay()
        {
            KeyPressReader.Cancel();
        }

        public RequestLanguage[] GetLanguages()
        {
            var result = new List<RequestLanguage>();
            foreach (var language in _consoleSettings.Languages)
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

        public void DisplayMessage(string value, int timeoutMilliseconds)
        {
            _logger.WriteLine(ConsoleColor.White, "DISPLAY: " + value);
            _logger.WriteLine(ConsoleColor.Gray, "DISPLAY: waiting {0} ms", timeoutMilliseconds);
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
                _logger.WriteLine(ConsoleColor.Cyan, "1AQ1KR : " + value);
                _logger.WriteLine(ConsoleColor.Gray, "1AQ1KR : waiting {0} ms", timeoutMilliseconds);
                _logger.Write(ConsoleColor.Cyan, "1AQ1KR : ");
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

        public void AppendReceipt(string value)
        {
            _logger.WriteLine(ConsoleColor.Magenta, "RECEIPT: " + value);
        }
    }
}
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
using TruRating.TruModule.ConsoleRunner.Environment;
using TruRating.TruModule.Device;

namespace TruRating.TruModule.ConsoleRunner.Device
{
    public class ConsoleReceiptManager :IReceiptManager
    {
        private readonly IConsoleLogger _consoleLogger;
        public ConsoleReceiptManager(IConsoleLogger consoleLogger)
        {
            _consoleLogger = consoleLogger;
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

        public void AppendReceipt(string value)
        {
            _consoleLogger.WriteLine(ConsoleColor.Magenta, "RECEIPT: " + value);
        }
    }
}
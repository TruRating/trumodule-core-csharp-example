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

namespace TruRating.TruModule.V2xx.Environment
{
    public interface IDevice
    {
        void PrintScreen(string value);
        void PrintReceipt(string value);
        void Log(string value, params object[] vars);
        void Error(string value, params object[] vars);
        ConsoleKeyInfo ReadKey(int timeoutMilliseconds);
    }

    public class ConsoleDevice : IDevice
    {
        private readonly ILogger _logger;

        public ConsoleDevice(ILogger logger)
        {
            _logger = logger;
        }

        public void PrintScreen(string value)
        {
            _logger.Write(ConsoleColor.White, "SCREEN :" + value);
        }

        public void PrintReceipt(string value)
        {
            _logger.Write(ConsoleColor.Magenta, "RECEIPT:" + value);
        }

        public void Log(string value, params object[] vars)
        {
            _logger.Write(ConsoleColor.DarkGray, "LOG    :" + value, vars);
        }

        public void Error(string value, params object[] vars)
        {
            _logger.Write(ConsoleColor.Red, "LOG    :" + value, vars);
        }

        public ConsoleKeyInfo ReadKey(int timeoutMilliseconds)
        {
            return KeyPressReader.ReadKey(timeoutMilliseconds);
        }
    }
}
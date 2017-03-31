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
using TruRating.TruModule.V2xx.Device;

namespace TruRating.TruModule.V2xx.ConsoleRunner.Environment
{
    public interface IConsoleIo : ILogger
    {
        void Write(ConsoleColor color, string value, params object[] vars);
        void WriteLine(ConsoleColor color, string value, params object[] vars);
        string ReadLine(string value);
    }

    public class ConsoleIo : IConsoleIo
    {
        private readonly object _consoleLock = new object();
        public string ReadLine(string value)
        {
            KeyPressReader.Stop();
            Write(ConsoleColor.Green, "CAPTURE: " + value + ": ");
            var readLine = Console.ReadLine();
            KeyPressReader.Start();
            return readLine;
        }
        public void WriteLine(ConsoleColor color, string value, params object[] vars)
        {
            lock (_consoleLock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(value, vars);
                Console.ResetColor();
            }
        }

        public void Write(ConsoleColor color, string value, params object[] vars)
        {
            lock (_consoleLock)
            {
                Console.ForegroundColor = color;
                Console.Write(value, vars);
                Console.ResetColor();
            }
        }

        public void Error(Exception e, string message)
        {
            WriteLine(ConsoleColor.Red, "{0}{1}", message, e);
        }

        public void Error(string format, params object[] parms)
        {
            WriteLine(ConsoleColor.Red, format, parms);
        }

        public void Warn(string format, params object[] parms)
        {
            WriteLine(ConsoleColor.Yellow, format, parms);
        }

        public void Info(string format, params object[] parms)
        {
            WriteLine(ConsoleColor.Green, format, parms);
        }

        public void Info(ConsoleColor consoleColor, string format, params object[] parms)
        {
            WriteLine(consoleColor, format, parms);
        }

        public void Debug(string format, params object[] parms)
        {
            WriteLine(ConsoleColor.Gray, format, parms);
        }
    }
}
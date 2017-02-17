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
using System.Threading;

namespace TruRating.TruModule.V2xx.Environment
{
    public delegate bool KeyPressEvenHandler(char key);

    internal class KeyPressReader
    {
        private static readonly Thread inputThread;
        private static volatile AutoResetEvent getInput, gotInput;
        private static ConsoleKeyInfo input;

        static KeyPressReader()
        {
            getInput = new AutoResetEvent(false);
            gotInput = new AutoResetEvent(false);
            inputThread = new Thread(reader);
            inputThread.IsBackground = true;
            inputThread.Start();
        }

        public static event KeyPressEvenHandler OnInputOverride;

        private static void reader()
        {
            var delegated = false;
            while (true)
            {
                if (!delegated)
                    getInput.WaitOne();
                input = Console.ReadKey(true);
                var invoke = false;
                if (OnInputOverride != null)
                {
                    invoke = OnInputOverride.Invoke(input.KeyChar);
                }
                if (invoke)
                {
                    delegated = true;
                }
                else
                {
                    delegated = false;
                    gotInput.Set();
                }
            }
        }

        public static ConsoleKeyInfo ReadKey()
        {
            return ReadKey(Timeout.Infinite);
        }

        public static ConsoleKeyInfo ReadKey(int timeOutMillisecs)
        {
            getInput.Set();
            var success = gotInput.WaitOne(timeOutMillisecs);
            if (success)
                return input;
            throw new TimeoutException("User did not provide input within the timelimit.");
        }
    }
}
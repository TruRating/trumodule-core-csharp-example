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
using TruRating.TruModule.V2xx.ConsoleRunner.Environment;
using TruRating.TruModule.V2xx.Device;

namespace TruRating.TruModule.V2xx.ConsoleRunner.UseCase
{
    public abstract class UseCaseBase : IUseCase
    {
        protected static readonly Random Rand = new Random();
        protected readonly IConsoleSettings ConsoleSettings;
        protected readonly IConsoleIo ConsoleIo;
        protected readonly IPinPad PinPad;
        protected readonly IPrinter Printer;
        protected UseCaseBase(IConsoleIo consoleIo, IConsoleSettings consoleSettings, IPinPad pinPad, IPrinter printer)
        {
            ConsoleIo = consoleIo;
            ConsoleSettings = consoleSettings;
            PinPad = pinPad;
            Printer = printer;
        }

        public void MainLoop()
        {
            ConsoleIo.WriteLine(ConsoleColor.Red, "Press any key to start");
            KeyPressReader.ReadKey();
            Init();
            while (true) //Endless loop
            {
                try
                {
                    Example();
                }
                catch (Exception e)
                {
                    ConsoleIo.WriteLine(ConsoleColor.DarkYellow, "Error {0}", e);
                }
                finally
                {
                    ConsoleIo.WriteLine(ConsoleColor.DarkGray, "");
                    ConsoleIo.WriteLine(ConsoleColor.DarkGray, "Waiting 1 second to finish");
                    Thread.Sleep(1000); //Wait for threads to join in TruModule
                    var endOfRunPressAKeyToReset = " End of run. Press a key to reset. ";
                    ConsoleIo.WriteLine(ConsoleColor.DarkGray,
                        endOfRunPressAKeyToReset.PadLeft(Console.WindowWidth - 1, '='));
                    ConsoleIo.WriteLine(ConsoleColor.DarkGray, "");
                    KeyPressReader.ReadKey();
                }
            }
        }

        public abstract void Example();

        public abstract bool IsApplicable();

        public abstract void Init();
    }
}
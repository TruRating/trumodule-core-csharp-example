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
using TruRating.TruModule.ConsoleRunner.Device;
using TruRating.TruModule.ConsoleRunner.Environment;
using TruRating.TruModule.ConsoleRunner.Settings;
using TruRating.TruModule.Device;

namespace TruRating.TruModule.ConsoleRunner.UseCase
{
    public abstract class UseCaseBase : IUseCase
    {
        protected static readonly Random Rand = new Random();
        protected readonly ConsoleSettings ConsoleSettings;
        protected readonly IConsoleLogger ConsoleLogger;
        protected readonly IDevice Device;
        protected readonly IReceiptManager ReceiptManager;
        protected UseCaseBase(IConsoleLogger consoleLogger, ConsoleSettings consoleSettings, IDevice device, IReceiptManager receiptManager)
        {
            ConsoleLogger = consoleLogger;
            ConsoleSettings = consoleSettings;
            Device = device;
            ReceiptManager = receiptManager;
        }

        public void MainLoop()
        {
            ConsoleLogger.WriteLine(ConsoleColor.Red, "Press any key to start");
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
                    ConsoleLogger.WriteLine(ConsoleColor.DarkYellow, "Error {0}", e);
                }
                finally
                {
                    ConsoleLogger.WriteLine(ConsoleColor.DarkGray, "");
                    ConsoleLogger.WriteLine(ConsoleColor.DarkGray, "Waiting 1 second to finish");
                    Thread.Sleep(1000); //Wait for threads to join in TruModule
                    var endOfRunPressAKeyToReset = " End of run. Press a key to reset. ";
                    ConsoleLogger.WriteLine(ConsoleColor.DarkGray,
                        endOfRunPressAKeyToReset.PadLeft(Console.WindowWidth - 1, '='));
                    ConsoleLogger.WriteLine(ConsoleColor.DarkGray, "");
                    KeyPressReader.ReadKey();
                }
            }
        }

        public abstract void Example();

        public abstract bool IsApplicable();

        public abstract void Init();
    }
}
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

using System.Collections.Generic;
using TruRating.TruModule.V2xx.ConsoleRunner.Environment;
using TruRating.TruModule.V2xx.Device;

namespace TruRating.TruModule.V2xx.ConsoleRunner.UseCase
{
    public class UseCaseFactory
    {
        internal static List<IUseCase> Get(IConsoleIo consoleIo, ConsoleSettings consoleSettings, IPinPad pinPad, IPrinter printer)
        {
            var standaloneUseCase = new StandaloneUseCase(consoleIo, consoleSettings, pinPad, printer);
            var integratedPosEventUseCase = new IntegratedPosEventUseCase(consoleIo, consoleSettings, pinPad, printer);
            var integratedPosEventListUseCase = new IntegratedPosEventListUseCase(consoleIo, consoleSettings, pinPad, printer);

            var modules = new List<IUseCase>
            {
                standaloneUseCase,
                integratedPosEventUseCase,
                integratedPosEventListUseCase
            };
            return modules;
        }
    }
}
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
using TruRating.TruModule.V2xx.Environment;
using TruRating.TruModule.V2xx.Scenarios;

namespace TruRating.TruModule.V2xx
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var logger = new Logger(true);
            try
            {
                var settings = new Settings(logger);
                var device = new ConsoleDevice(logger);

                IExemplar module = null;
                foreach (var truModule in ExemplarFactory.Get(logger, settings, device))
                {
                    if (truModule.IsApplicable())
                    {
                        module = truModule;
                        break;
                    }
                }
                if (module != null)
                {
                    module.RunLoop();
                }
                else
                {
                    throw new Exception("Could not find a TruModule implementation for the specified settings");
                }
            }
            catch (Exception e)
            {
                logger.Write(ConsoleColor.Red, e.Message);
                logger.Write(ConsoleColor.Red, "Press enter to exit");
                KeyPressReader.ReadKey();
            }
        }
    }
}
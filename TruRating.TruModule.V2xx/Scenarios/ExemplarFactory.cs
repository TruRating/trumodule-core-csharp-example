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
using TruRating.TruModule.V2xx.Environment;
using TruRating.TruModule.V2xx.Messages;

namespace TruRating.TruModule.V2xx.Scenarios
{
    public class ExemplarFactory
    {
        internal static List<IExemplar> Get(Logger logger, Settings settings, ConsoleDevice consoleDevice)
        {
            var httpClient = new HttpClient(settings, logger, new MacSignatureCalculator());
            var tsiMessages = new TsiMessages(settings, logger, httpClient);

            var truModuleV200 = new V200Exemplar(logger, settings, consoleDevice, tsiMessages.V200);
            var truModuleV210 = new V210NoPosIntegrationExemplar(logger, settings, consoleDevice, tsiMessages.V200);
            var truModuleV210PosEventList = new V210PosEventListExemplar(logger, settings, consoleDevice,
                tsiMessages.V210);
            var truModuleV210PosEvents = new V210PosEventsExemplar(logger, settings, consoleDevice, tsiMessages.V210);
            var truModuleV220 = new V220Exemplar(logger, settings, consoleDevice, tsiMessages.V220, truModuleV200,
                truModuleV210PosEvents, truModuleV210PosEventList);

            var modules = new List<IExemplar>
            {
                truModuleV200,
                truModuleV210,
                truModuleV210PosEventList,
                truModuleV210PosEvents,
                truModuleV220
            };
            return modules;
        }
    }
}
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
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Environment;

namespace TruRating.TruModule.V2xx.Messages
{
    public class TsiV200Messages : ITsiV200Messages
    {
        private static readonly Random Rand = new Random();
        protected readonly IHttpClient HttpClient;
        protected readonly ILogger Logger;
        protected readonly ISettings Settings;

        public TsiV200Messages(ISettings settings, ILogger logger, IHttpClient httpClient)
        {
            Settings = settings;
            Logger = logger;
            HttpClient = httpClient;
        }

        public Response SendRequestQuestion(List<RequestLanguage> languages, string sessionId, Trigger trigger)
        {
            Logger.WriteLine(ConsoleColor.Green, "LOGIC : Requesting question"); //Debug
            var request = CreateBlankRequest(sessionId);
            request.Item = new RequestQuestion
            {
                Trigger = trigger,
                Device = CreateRequestDevice(),
                Language = languages.ToArray(), //Ask for 2 languages
                Server = CreateRequestServer()
            };
            return HttpClient.Send(request);
        }

        public Response SendRequestTransaction(string sessionId)
        {
            var request = CreateBlankRequest(sessionId);
            request.Item = new RequestTransaction //Prepare the dummy transaction
            {
                Amount = Rand.Next(1000, 2000),
                Currency = 826,
                DateTime = DateTime.UtcNow, //.ToString("O"),
                Gratuity = 0,
                Id = Guid.NewGuid().ToString(),
                Result = TransactionResult.APPROVED
            };
            return HttpClient.Send(request);
        }

        public Response SendRequestRating(string sessionId, RequestRating requestRating)
        {
            var request = CreateBlankRequest(sessionId);
            request.Item = requestRating;
            return HttpClient.Send(request);
        }

        protected RequestServer CreateRequestServer()
        {
            return new RequestServer
            {
                Id = "ServerName",
                Firmware = Settings.Version
            };
        }

        protected RequestDevice CreateRequestDevice()
        {
            return new RequestDevice
            {
                Name = Settings.TerminalId,
                Firmware = Settings.Version,
                Screen = new RequestPeripheral
                {
                    Separator = "|",
                    Font = Font.MONOSPACED,
                    Format = Format.TEXT,
                    Height = 4,
                    HeightSpecified = true,
                    Width = 16,
                    WidthSpecified = true,
                    Unit = UnitDimension.LINE
                },
                SkipInstruction = SkipInstruction.CLRMIXED,
                SkipInstructionSpecified = true
            };
        }

        protected RequestLanguage[] CreateRequestLanguage()
        {
            var result = new List<RequestLanguage>();
            foreach (var language in Settings.Languages)
            {
                result.Add(new RequestLanguage {Rfc1766 = language});
            }
            return result.ToArray();
        }

        protected Request CreateBlankRequest(string sessionId)
        {
            return new Request
            {
                SessionId = sessionId,
                TerminalId = Settings.TerminalId,
                MerchantId = Settings.MerchantId,
                PartnerId = Settings.PartnerId
            };
        }
    }
}
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
using TruRating.TruModule.V2xx.Messages;

namespace TruRating.TruModule.V2xx.Scenarios
{
    public class V200Exemplar : ExemplarBase
    {
        private readonly ITsiV200Messages _tsiV200Messages;


        public V200Exemplar(ILogger logger, ISettings settings, IDevice device, ITsiV200Messages tsiV200Messages)
            : base(logger, settings, device)
        {
            _tsiV200Messages = tsiV200Messages;
        }

        public override void Scenario()
        {
            var sessionId = Rand.Next(1, 1000000).ToString(); //Randomize a new session
            var language = Settings.Languages[Rand.Next(0, Settings.Languages.Length)]; //Pick a random language
            var languages = new List<RequestLanguage>();
            foreach (var rfc1766 in Settings.Languages)
            {
                languages.Add(new RequestLanguage
                {
                    Rfc1766 = rfc1766
                });
            }
            var response = _tsiV200Messages.SendRequestQuestion(languages, sessionId, Trigger.PAYMENTREQUEST);
            if (response.Item is ResponseDisplay)
            {
                var rating = CaptureRating(response, language);
                if (rating != null)
                {
                    Logger.WriteLine(ConsoleColor.Green, "LOGIC : Had question to ask, rating captured as {0}, sending rating",
                        rating.Value);
                    _tsiV200Messages.SendRequestRating(sessionId, rating);
                    _tsiV200Messages.SendRequestTransaction(sessionId);
                    return;
                }
            }
            Logger.WriteLine(ConsoleColor.Green, "LOGIC : No question to ask, continuing");
            _tsiV200Messages.SendRequestTransaction(sessionId);
        }

        public override bool IsApplicable()
        {
            return Settings.TsiVersion == TsiVersion.V200;
        }
    }
}
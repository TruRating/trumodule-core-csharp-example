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
    public class V210PosEventsExemplar : ExemplarBase
    {
        private readonly ITsiV210Messages _tsiV210Messages;


        public V210PosEventsExemplar(ILogger logger, ISettings settings, IDevice device,
            ITsiV210Messages tsiV210Messages) : base(logger, settings, device)
        {
            _tsiV210Messages = tsiV210Messages;
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
            var log = new List<Type>();
            while (true) //PosIntegration
            {
                var response = _tsiV210Messages.SendRequestPosEvent(sessionId, log);
                if (response == null)
                    break;
                var item = response.Item as ResponseEvent;
                if (item != null)
                {
                    Logger.Write(ConsoleColor.Green, "Received an event"); //Debug
                    var question = item.Item as ResponseEventQuestion;
                    if (question != null)
                    {
                        var events = question;
                        response = _tsiV210Messages.SendRequestQuestion(languages, sessionId, events.Trigger);
                        if (response.Item is ResponseDisplay)
                        {
                            var rating = CaptureRating(response, language);
                            if (rating != null)
                            {
                                Logger.Write(ConsoleColor.Green,
                                    "Had question to ask, rating captured as {0}, sending rating", rating.Value);
                                _tsiV210Messages.SendRequestRating(sessionId, rating);
                            }
                        }
                        else
                        {
                            Logger.Write(ConsoleColor.Green, "No question to ask, continuing");
                        }
                    }
                }
                if (log.Count > 0 && log[log.Count - 1] == typeof (RequestPosEndTransaction))
                {
                    _tsiV210Messages.SendRequestTransaction(sessionId);
                }
            }
        }

        public override bool IsApplicable()
        {
            return Settings.TsiVersion == TsiVersion.V210 && Settings.PosIntegration == PosIntegration.Events;
        }
    }
}
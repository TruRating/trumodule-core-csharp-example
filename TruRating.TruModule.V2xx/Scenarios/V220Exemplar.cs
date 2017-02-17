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
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Environment;
using TruRating.TruModule.V2xx.Messages;

namespace TruRating.TruModule.V2xx.Scenarios
{
    public class V220Exemplar : ExemplarBase
    {
        private readonly IDevice _device;
        private readonly ITsiV220Messages _tsiV220Messages;
        private readonly V200Exemplar _v200Exemplar;
        private readonly V210PosEventListExemplar _v210PosEventListExemplar;
        private readonly V210PosEventsExemplar _v210PosEventsExemplar;
        private Boolean _forceQuery = true;

        public V220Exemplar(ILogger logger, ISettings settings, IDevice device, ITsiV220Messages tsiV220Messages,
            V200Exemplar v200Exemplar, V210PosEventsExemplar v210PosEventsExemplar,
            V210PosEventListExemplar v210PosEventListExemplar) : base(logger, settings, device)
        {
            _device = device;
            _tsiV220Messages = tsiV220Messages;
            _v200Exemplar = v200Exemplar;
            _v210PosEventsExemplar = v210PosEventsExemplar;
            _v210PosEventListExemplar = v210PosEventListExemplar;
        }

        public override void Scenario()
        {
            if (IsActivated())
            {
                switch (Settings.PosIntegration)
                {
                    case PosIntegration.None:
                        _v200Exemplar.Scenario();
                        break;
                    case PosIntegration.Events:
                        _v210PosEventsExemplar.Scenario();
                        break;
                    case PosIntegration.EventList:
                        _v210PosEventListExemplar.Scenario();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                Lookups();
                Activate();
            }
        }

        public override bool IsApplicable()
        {
            return Settings.TsiVersion == TsiVersion.V220;
        }

        private void Lookups()
        {
            if (Settings.RegistrationCode)
                return;
            var status = _tsiV220Messages.SendRequestLookup();
            var responseStatus = status.Item as ResponseLookup;
            if (responseStatus != null)
            {
                foreach (var language in responseStatus.Language)
                {
                    if (language.Option != null)
                        foreach (var lookupOption in language.Option)
                        {
                            PrintLookups(lookupOption, 1);
                        }
                }
            }
        }

        private void PrintLookups(LookupOption lookupOption, int depth)
        {
            _device.PrintScreen("".PadLeft(depth, ' ') + lookupOption.Text + " (" + lookupOption.Value + ")");
            if (lookupOption.Option != null)
                foreach (var option in lookupOption.Option)
                {
                    PrintLookups(option, depth + 1);
                }
        }

        private void Activate()
        {
            var status = _tsiV220Messages.SendRequestActivate();
            var responseStatus = status.Item as ResponseStatus;
            if (responseStatus != null)
            {
                Settings.ActivationRecheck = DateTime.UtcNow.AddMinutes(responseStatus.TimeToLive);
                Settings.IsActivated = responseStatus.IsActive;
            }
        }

        private bool IsActivated()
        {
            if (Settings.ActivationRecheck > DateTime.UtcNow)
            {
                return Settings.IsActivated;
            }
            var status = _tsiV220Messages.SendRequestQuery(_forceQuery);
            var responseStatus = status.Item as ResponseStatus;
            if (responseStatus != null)
            {
                if (_forceQuery)
                {
                    _forceQuery = false;
                }
                Settings.ActivationRecheck = DateTime.UtcNow.AddSeconds(responseStatus.TimeToLive);
                Settings.IsActivated = responseStatus.IsActive;
            }
            return Settings.IsActivated;
        }
    }
}
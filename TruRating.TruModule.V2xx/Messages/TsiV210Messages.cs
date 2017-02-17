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
    public class TsiV210Messages : TsiV200Messages, ITsiV210Messages
    {
        public TsiV210Messages(ISettings settings, ILogger logger, IHttpClient httpClient)
            : base(settings, logger, httpClient)
        {
        }

        public Response SendRequestPosEventList(string sessionId)
        {
            var request = CreateBlankRequest(sessionId);
            request.Item = new RequestPosEventList
            {
                StartTransaction = CreateRequestPosStartTransaction(),
                StartTilling = CreateRequestPosStartTilling(),
                Items = new object[] {CreateRequestPosItem()},
                EndTilling = CreateRequestPosEndTilling(),
                EndTransaction = CreateRequestPosEndTransaction()
            };
            return HttpClient.Send(request);
        }


        public Response SendRequestPosEvent(string sessionId, ICollection<Type> log)
        {
            object posEvent = null;
            //Make all pos events
            //This creates a single pos event of each type in order - this is the minimum number of events expected by TruService
            if (!log.Contains(typeof (RequestPosStartTransaction)))
            {
                posEvent = CreateRequestPosStartTransaction();
            }
            else if (!log.Contains(typeof (RequestPosStartTilling)))
            {
                posEvent = CreateRequestPosStartTilling();
            }
            else if (!log.Contains(typeof (RequestPosItem)))
            {
                posEvent = CreateRequestPosItem();
            }
            else if (!log.Contains(typeof (RequestPosEndTilling)))
            {
                posEvent = CreateRequestPosEndTilling();
            }
            else if (!log.Contains(typeof (RequestPosEndTransaction)))
            {
                posEvent = CreateRequestPosEndTransaction();
            }
            if (posEvent != null)
            {
                var posEventRequest = CreateBlankRequest(sessionId);
                posEventRequest.Item = new RequestPosEvent
                {
                    Items = new[] {posEvent}
                };
                log.Add(posEvent.GetType());
                return HttpClient.Send(posEventRequest);
            }
            return null;
        }

        private RequestPosStartTilling CreateRequestPosStartTilling()
        {
            var posEvent = new RequestPosStartTilling();
            return posEvent;
        }

        private RequestPosStartTransaction CreateRequestPosStartTransaction()
        {
            var posEvent = new RequestPosStartTransaction
            {
                AttendantType = AttendantType.ATTENDED,
                AttendantTypeSpecified = true,
                OperatorId = "James",
                SalesPersonId = "James",
                TillType = "SCO"
            };
            return posEvent;
        }

        private RequestPosEndTilling CreateRequestPosEndTilling()
        {
            var posEvent = new RequestPosEndTilling
            {
                SubTotalAmount = 640
            };
            return posEvent;
        }

        private RequestPosEndTransaction CreateRequestPosEndTransaction()
        {
            var posEvent = new RequestPosEndTransaction();
            return posEvent;
        }

        private RequestPosItem CreateRequestPosItem()
        {
            var posEvent = new RequestPosItem
            {
                Quantity = 1
            };
            return posEvent;
        }
    }
}
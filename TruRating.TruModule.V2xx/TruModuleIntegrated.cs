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
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Device;
using TruRating.TruModule.V2xx.Messages;
using TruRating.TruModule.V2xx.Network;
using TruRating.TruModule.V2xx.Settings;
using TruRating.TruModule.V2xx.Util;

namespace TruRating.TruModule.V2xx
{
    public class TruModuleIntegrated : TruModule, ITruModuleIntegrated
    {
        public TruModuleIntegrated(IDevice device, IReceiptManager receiptManager, ITruServiceClient truServiceClient, ILogger logger,
            ITruServiceMessageFactory truServiceMessageFactory, ISettings settings)
            : base(device,receiptManager, truServiceClient, logger, truServiceMessageFactory, settings)
        {
        }

        public void SendTransaction(PosParams posParams, RequestTransaction requestTransaction)
        {
            if (IsActivated(bypassTruServiceCache:false))
            {
                var request = TruServiceMessageFactory.AssembleRequestTransaction(posParams.PartnerId,
                    posParams.SessionId, posParams.MerchantId, posParams.TerminalId, requestTransaction);
                SendRequest(request);
            }
        }

        public void SendPosEvent(PosParams posParams, RequestPosEvent requestPosEvent)
        {
            if (IsActivated(bypassTruServiceCache:false))
            {
                TaskHelpers.BeginTask(() =>
                {
                    var request = TruServiceMessageFactory.AssembleRequestPosEvent(posParams, requestPosEvent);
                    var response = SendRequest(request);
                    var item = response != null && response.Item is ResponseEvent
                        ? ((ResponseEvent) response.Item).Item
                        : null;
                    if (item is ResponseEventQuestion)
                    {
                        Settings.Trigger = (item as ResponseEventQuestion).Trigger;
                        if (Settings.Trigger == Trigger.DWELLTIME || Settings.Trigger == Trigger.DWELLTIMEEXTEND)
                        {
                            var questionRequest = TruServiceMessageFactory.AssembleRequestQuestion(Device,ReceiptManager,posParams.PartnerId, posParams.MerchantId, posParams.TerminalId, posParams.SessionId, Settings.Trigger);
                            DoRating(questionRequest);
                        }
                    }
                    else if (item is ResponseEventClear)
                    {
                        CancelRating();
                    }
                    return 1;
                });
            }
        }

        public void SendPosEventList(PosParams posParams, RequestPosEventList requestPosEventList)
        {
            if (IsActivated(bypassTruServiceCache:false))
            {
                TaskHelpers.BeginTask(() =>
                {
                    var request = TruServiceMessageFactory.AssembleRequestPosEvent(posParams, requestPosEventList);
                    SendRequest(request);
                    return 1;
                });
            }
        }

        public void InitiatePayment(PosParams posParams)
        {
            if (IsActivated(bypassTruServiceCache:false))
            {
                if (Settings.Trigger == Trigger.PAYMENTREQUEST)
                {
                    var questionRequest = TruServiceMessageFactory.AssembleRequestQuestion(Device,ReceiptManager, posParams.PartnerId,
                        posParams.MerchantId, posParams.TerminalId, posParams.SessionId, Settings.Trigger);
                    DoRating(questionRequest);
                }
                else
                {
                    CancelRating();
                }
            }
        }
    }
}
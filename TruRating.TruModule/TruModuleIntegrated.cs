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
using TruRating.TruModule.Device;
using TruRating.TruModule.Messages;
using TruRating.TruModule.Network;
using TruRating.TruModule.Settings;
using TruRating.TruModule.Util;

namespace TruRating.TruModule
{
    public class TruModuleIntegrated : TruModule, ITruModuleIntegrated
    {
        /// <summary>
        /// Create instance of TruModule with the default implementations of ITruServiceClient, ITruServiceMessageFactory and related dependencies
        /// </summary>
        public TruModuleIntegrated(ILogger logger, ISettings settings, IDevice device, IReceiptManager receiptManager)
            : this(logger, settings, device, receiptManager, TruServiceHttpClient.CreateDefault(logger, settings), Messages.TruServiceMessageFactory.CreateDefault())
        {
        }
        protected TruModuleIntegrated(ILogger logger, ISettings settings,IDevice device, IReceiptManager receiptManager, ITruServiceClient truServiceClient,
            ITruServiceMessageFactory truServiceMessageFactory)
            : base(logger, settings, device, receiptManager, truServiceClient, truServiceMessageFactory)
        {
        }

        public void SendTransaction(PosParams posParams, RequestTransaction requestTransaction)
        {
            try
            {
                if (IsActivated(bypassTruServiceCache: false))
                {
                    var request = TruServiceMessageFactory.AssembleRequestTransaction(new RequestParams(posParams), requestTransaction);
                    TaskHelpers.BeginTask(() =>
                    {
                        return SendRequest(request);
                    });
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "TruModuleIntegrated - Error in SendTransaction");
            }
        }

        public void SendPosEvent(PosParams posParams, RequestPosEvent requestPosEvent)
        {
            try
            {
                if (IsActivated(bypassTruServiceCache: false))
                {
                    TaskHelpers.BeginTask(() =>
                    {
                        try
                        {
                            var request = TruServiceMessageFactory.AssembleRequestPosEvent(posParams, requestPosEvent);
                            var response = SendRequest(request);
                            var item = response != null && response.Item is ResponseEvent
                                ? ((ResponseEvent)response.Item).Item
                                : null;
                            if (item is ResponseEventQuestion)
                            {
                                Settings.Trigger = (item as ResponseEventQuestion).Trigger;
                                if (Settings.Trigger == Trigger.DWELLTIME || Settings.Trigger == Trigger.DWELLTIMEEXTEND)
                                {
                                    var questionRequest = TruServiceMessageFactory.AssembleRequestQuestion(new RequestParams(posParams), Device, ReceiptManager, Settings.Trigger);
                                    DoRating(questionRequest);
                                }
                            }
                            else if (item is ResponseEventClear)
                            {
                                CancelRating();
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e, "TruModuleIntegrated - Error in SendPosEvent Task");
                        }
                        return 1;
                    });
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "TruModuleIntegrated - Error in SendPosevent");
            }
        }



        public void InitiatePayment(PosParams posParams)
        {
            try
            {
                if (IsActivated(bypassTruServiceCache: false))
                {
                    if (Settings.Trigger == Trigger.PAYMENTREQUEST)
                    {
                        var questionRequest = TruServiceMessageFactory.AssembleRequestQuestion(new RequestParams(posParams), Device, ReceiptManager, Settings.Trigger);
                        DoRating(questionRequest);
                    }
                    else
                    {
                        CancelRating();
                    }
                }
            }
            catch(Exception e)
            {
                Logger.Error(e, "TruModuleIntegrated - Error in InitiatePayment");
            }
        }
    }
}
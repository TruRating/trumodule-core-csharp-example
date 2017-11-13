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
using TruRating.TruModule.Device;

namespace TruRating.TruModule.Messages
{
    public class TruServiceMessageFactory : ITruServiceMessageFactory
    {
        internal static ITruServiceMessageFactory CreateDefault()
        {
            return new TruServiceMessageFactory();
        }
        public Request AssemblyRequestQuery(RequestParams requestParams, IDevice device, IReceiptManager receiptManager,
            bool force)
        {
            var result = new Request
            {
                PartnerId = requestParams.PartnerId,
                MerchantId = requestParams.MerchantId,
                TerminalId = requestParams.TerminalId,
                SessionId = requestParams.SessionId,
                Item = new RequestQuery
                {
                    Device = new RequestDevice
                    {
                        Name = device.GetName(),
                        Firmware = device.GetFirmware(),
                        Screen = device.GetScreenCapabilities(),
                        SkipInstruction = device.GetSkipInstruction(),
                        SkipInstructionSpecified = true,
                        Receipt = receiptManager.GetReceiptCapabilities(),
                    },
                    Language = device.GetLanguages(),
                    Server = device.GetServer(),
                    ForceSpecified = true,
                    Force = force
                }
            };
            return result;
        }

        public Request AssembleRequestRating(RequestParams requestParams, RequestRating rating)
        {
            var result = new Request
            {
                PartnerId = requestParams.PartnerId,
                MerchantId = requestParams.MerchantId,
                TerminalId = requestParams.TerminalId,
                SessionId = requestParams.SessionId,
                Item = rating
            };
            return result;
        }

        public Request AssembleRequestQuestion(RequestParams requestParams,
            IDevice device, IReceiptManager receiptManager,
            Trigger trigger)
        {
            var result = new Request
            {
                PartnerId = requestParams.PartnerId,
                MerchantId = requestParams.MerchantId,
                TerminalId = requestParams.TerminalId,
                SessionId = requestParams.SessionId,
                Item = new RequestQuestion
                {
                    Trigger = trigger,
                    Device = new RequestDevice
                    {
                        Name = device.GetName(),
                        Firmware = device.GetFirmware(),
                        Screen = device.GetScreenCapabilities(),
                        SkipInstruction = device.GetSkipInstruction(),
                        SkipInstructionSpecified  = true,
                        Receipt = receiptManager.GetReceiptCapabilities(),
                    },
                    Language = device.GetLanguages(),
                    Server = device.GetServer()
                }
            };
            return result;
        }

        public Request AssembleRequestTransaction(RequestParams requestParams, RequestTransaction requestTransaction)
        {
            var result = new Request
            {
                PartnerId = requestParams.PartnerId,
                MerchantId = requestParams.MerchantId,
                TerminalId = requestParams.TerminalId,
                SessionId = requestParams.SessionId,
                Item = requestTransaction
            };
            return result;
        }

        public Request AssembleRequestPosEvent(PosParams posParams, RequestPosEvent requestPosEvent)
        {
            var result = new Request
            {
                PartnerId = posParams.PartnerId,
                MerchantId = posParams.MerchantId,
                TerminalId = posParams.TerminalId,
                SessionId = posParams.SessionId,
                Item = requestPosEvent
            };
            return result;
        }

        public Request AssembleRequestPosEventList(RequestParams requestParams, RequestPosEventList requestPosEventList)
        {
            var result = new Request
            {
                PartnerId = requestParams.PartnerId,
                MerchantId = requestParams.MerchantId,
                TerminalId = requestParams.TerminalId,
                SessionId = requestParams.SessionId,
                Item = requestPosEventList
            };
            return result;
        }

        public Request AssembleRequestLookup(RequestParams requestParams, IDevice device, IReceiptManager receiptManager, LookupName lookupName)
        {
            var result = new Request
            {
                PartnerId = requestParams.PartnerId,
                MerchantId = requestParams.MerchantId,
                TerminalId = requestParams.TerminalId,
                SessionId = requestParams.SessionId,
                Item = new RequestLookup
                {
                    Name = lookupName,
                    Device = new RequestDevice
                    {
                        Name = device.GetName(),
                        Firmware = device.GetFirmware(),
                        Screen = device.GetScreenCapabilities(),
                        SkipInstruction = device.GetSkipInstruction(),
                        SkipInstructionSpecified = true,
                        Receipt = receiptManager.GetReceiptCapabilities(),
                    },
                    Language = device.GetLanguages(),
                    Server = device.GetServer()
                }
            };
            return result;
        }

        public Request AssembleRequestActivate(RequestParams requestParams, IDevice device, IReceiptManager receiptManager, int sectorNode, string timeZone, PaymentInstant paymentInstant, string emailAddress,
            string password, string address, string mobileNumber, string merchantName, string businessName)
        {
            var result = new Request
            {
                PartnerId = requestParams.PartnerId,
                MerchantId = requestParams.MerchantId,
                TerminalId = requestParams.TerminalId,
                SessionId = requestParams.SessionId,
                Item = new RequestActivate
                {
                    Device = new RequestDevice
                    {
                        Name = device.GetName(),
                        Firmware = device.GetFirmware(),
                        Screen = device.GetScreenCapabilities(),
                        SkipInstruction = device.GetSkipInstruction(),
                        SkipInstructionSpecified = true,
                        Receipt = receiptManager.GetReceiptCapabilities(),
                    },
                    Language = device.GetLanguages(),
                    Server = device.GetServer(),
                    Item = new RequestRegistrationForm
                    {
                        BusinessAddress = address,
                        BusinessName = businessName,
                        MerchantEmailAddress = emailAddress,
                        MerchantMobileNumber = mobileNumber,
                        MerchantName = merchantName,
                        MerchantPassword = password,
                        PaymentInstant = paymentInstant,
                        SectorNode = sectorNode,
                        TimeZone = timeZone
                    }
                }
            };
            return result;
        }

        public Request AssembleRequestActivate(RequestParams requestParams, IDevice device, IReceiptManager receiptManager,string registrationCode)
        {
            var result = new Request
            {
                PartnerId = requestParams.PartnerId,
                MerchantId = requestParams.MerchantId,
                TerminalId = requestParams.TerminalId,
                SessionId = requestParams.SessionId,
                Item = new RequestActivate
                {
                    Device = new RequestDevice
                    {
                        Name = device.GetName(),
                        Firmware = device.GetFirmware(),
                        Screen = device.GetScreenCapabilities(),
                        SkipInstruction = device.GetSkipInstruction(),
                        SkipInstructionSpecified = true,
                        Receipt = receiptManager.GetReceiptCapabilities(),
                    },
                    Language = device.GetLanguages(),
                    Server = device.GetServer(),
                    Item = registrationCode
                }
            };
            return result;
        }

        
    }
}
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
using Rhino.Mocks;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.Device;
using TruRating.TruModule.Network;
using TruRating.TruModule.Tests.Unit.TruModuleTests;

namespace TruRating.TruModule.Tests.Unit.TruModuleStandaloneTests.Lookup
{
    public class TruModuleStandaloneLookupTestContext: MsTestsContext<TestContextTruModuleStandalone>
    {
        protected IDevice Device { get; set; }
        protected ITruServiceClient TruServiceClient { get; set; }

        protected ResponseLookupLanguage ResponseLookupLanguage;
        protected LookupOption[] LookupsReturned;

        protected string ResponseLanguageCode { get; set; }
        protected string DeviceLanguageCode { get; set; }

        public void Setup()
        {
            Device = MockOf<IDevice>();
            TruServiceClient = MockOf<ITruServiceClient>();

            Device.Stub(t => t.GetCurrentLanguage()).Return(DeviceLanguageCode);

            ResponseLookupLanguage = CreateLookupLanguageResposne(ResponseLanguageCode);


            TruServiceClient.Stub(t => t.Send(Arg<Request>.Is.Anything)).Return(new Response()
            {
                Item = new ResponseLookup()
                {
                    Language = new[]
                    {
                        ResponseLookupLanguage
                    }
                }
            });
        }

        public static ResponseLookupLanguage CreateLookupLanguageResposne(string rfc1766LanguageCode)
        {
            var lookupLanguage = new ResponseLookupLanguage()
            {
                Rfc1766 = rfc1766LanguageCode,
                Option = new[]
                {
                    new LookupOption()
                    {
                        Text = "Sector 1",
                        Value = "1"
                    },
                    new LookupOption()
                    {
                        Text = "Sector 2",
                        Value = "2"
                    },
                }
            };
            return lookupLanguage;
        }
    }
}
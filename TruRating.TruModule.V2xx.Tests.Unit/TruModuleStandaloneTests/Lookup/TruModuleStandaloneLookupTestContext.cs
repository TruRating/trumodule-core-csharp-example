using Rhino.Mocks;
using TruRating.Dto.TruService.V220;
using TruRating.TruModule.V2xx.Device;
using TruRating.TruModule.V2xx.Network;

namespace TruRating.TruModule.V2xx.Tests.Unit.TruModuleStandaloneTests.Lookup
{
    public class TruModuleStandaloneLookupTestContext: MsTestsContext<TruModuleStandalone>
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
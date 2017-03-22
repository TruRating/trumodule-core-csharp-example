using TruRating.Dto.TruService.V220;

namespace TruRating.TruModule.V2xx.Device
{
    public interface IPrinter
    {
        RequestPeripheral GetReceiptCapabilities();
        void AppendReceipt(string value);
    }
}
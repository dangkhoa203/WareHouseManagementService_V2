using NanoidDotNet;
using WareHouseManagement.Model.Receipt;

namespace WareHouseManagement.Model.Form
{
    public class ReturnBuyForm: FormGeneric
    {
        public ReturnBuyForm()
        {
            Id= $"HOANTIEN-{Nanoid.Generate(Nanoid.Alphabets.LowercaseLettersAndDigits, 5)}";
        }
        public  DateTime OrderDate { get; set; }
        public string ReceiptId { get; set; }

    }
}

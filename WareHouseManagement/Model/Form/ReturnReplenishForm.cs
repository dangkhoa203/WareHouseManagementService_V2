using NanoidDotNet;
using WareHouseManagement.Model.Receipt;

namespace WareHouseManagement.Model.Form
{
    public class ReturnReplenishForm: FormGeneric
    {
        public ReturnReplenishForm()
        {
            Id= $"TRAHANG-{Nanoid.Generate(Nanoid.Alphabets.LowercaseLettersAndDigits, 5)}";
        }
        public DateTime OrderDate { get; set; }
        public string ReceiptId { get; set; }

    }
}

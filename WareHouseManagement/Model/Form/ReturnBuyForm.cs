using NanoidDotNet;
using WareHouseManagement.Model.Entity;
using WareHouseManagement.Model.Receipt;

namespace WareHouseManagement.Model.Form
{
    public class ReturnBuyForm: EntityGeneric
    {
        public ReturnBuyForm()
        {
            Id= $"HOANTIEN-{Nanoid.Generate(Nanoid.Alphabets.LowercaseLettersAndDigits, 5)}";
        }
        public  DateTime OrderDate { get; set; }
        public string ReceiptId { get; set; }

    }
}

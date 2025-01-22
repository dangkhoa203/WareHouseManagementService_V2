using NanoidDotNet;
using WareHouseManagement.Model.Entity.Vendor_EntiTy;
using WareHouseManagement.Model.Form;

namespace WareHouseManagement.Model.Receipt
{
    public class VendorReplenishReceipt:ReceiptGeneric
    {
        public VendorReplenishReceipt()
        {
            Id= $"HDNHAPHANG-{Nanoid.Generate(Nanoid.Alphabets.LowercaseLettersAndDigits, 5)}";
        }
        public DateTime DateOrder {  get; set; }

        public virtual StockImportForm? StockImportReport { get; set; }
        public virtual Vendor Vendor { get; set; }
        public virtual ICollection<VendorReplenishReceiptDetail> Details { get; set; }
    }
}

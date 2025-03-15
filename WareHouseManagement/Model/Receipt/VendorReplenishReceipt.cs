using NanoidDotNet;
using WareHouseManagement.Model.Entity.Generic;
using WareHouseManagement.Model.Entity.Vendor_EntiTy;
using WareHouseManagement.Model.Enum;
using WareHouseManagement.Model.Form;

namespace WareHouseManagement.Model.Receipt
{
    public class VendorReplenishReceipt:EntityGeneric
    {
        public required DateTime DateOrder { get; set; }
        public required float ReceiptValue { get; set; }
        public StatusEnum Status { get; set; }
        public VendorReplenishReceipt()
        {
            Id= $"HDNHAPHANG-{Nanoid.Generate(Nanoid.Alphabets.LowercaseLettersAndDigits, 5)}";
            Status = StatusEnum.Draft;
        }
        public virtual Tax? Tax { get; set; }
        public virtual ICollection<ImportForm> StockImportReports { get; set; }
        public virtual Vendor Vendor { get; set; }
        public virtual ICollection<VendorReplenishReceiptDetail> Details { get; set; }
    }
}

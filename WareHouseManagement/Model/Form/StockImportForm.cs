using NanoidDotNet;
using WareHouseManagement.Model.Entity;
using WareHouseManagement.Model.Entity.Generic;
using WareHouseManagement.Model.Enum;
using WareHouseManagement.Model.Receipt;

namespace WareHouseManagement.Model.Form
{
    public class StockImportForm: EntityGeneric
    {
        public string ReceiptId { get; set; }
        public required DateTime ImportDate { get; set; }
        public StatusEnum Status { get; set; }
        public StockImportForm():base()
        {
            Id = $"NHAPKHO-{Nanoid.Generate(Nanoid.Alphabets.LowercaseLettersAndDigits, 5)}";
            Status = StatusEnum.Draft;
        }
       
        public virtual VendorReplenishReceipt? Receipt { get; set; }
        public virtual ICollection<ImportFormDetail>? Details { get; set; }
    }
}

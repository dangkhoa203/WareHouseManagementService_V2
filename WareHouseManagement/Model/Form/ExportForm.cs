using NanoidDotNet;
using WareHouseManagement.Model.Entity;
using WareHouseManagement.Model.Entity.Generic;
using WareHouseManagement.Model.Enum;
using WareHouseManagement.Model.Receipt;

namespace WareHouseManagement.Model.Form
{
    public class ExportForm: EntityGeneric {
        public string ReceiptId { get; set; }
        public required DateTime ExportDate { get; set; }
        public StatusEnum Status { get; set; }
        public ExportForm():base()
        {
            Id= $"XUATKHO-{Nanoid.Generate(Nanoid.Alphabets.LowercaseLettersAndDigits, 5)}";
            Status = StatusEnum.Draft ;
        }
        public virtual CustomerBuyReceipt Receipt { get; set; }
        public virtual ICollection<ExportFormDetail> Details { get; set; }
    }
}

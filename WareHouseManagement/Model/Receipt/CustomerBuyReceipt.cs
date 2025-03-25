using Microsoft.EntityFrameworkCore;
using NanoidDotNet;
using WareHouseManagement.Model.Entity.Customer_Entity;
using WareHouseManagement.Model.Entity.Generic;
using WareHouseManagement.Model.Enum;
using WareHouseManagement.Model.Form;

namespace WareHouseManagement.Model.Receipt
{
    public class CustomerBuyReceipt:EntityGeneric
    {
        public required DateTime DateOrder { get; set; }
        public required float ReceiptValue { get; set; }
        public StatusEnum Status { get; set; }
        public CustomerBuyReceipt()
        {
            Id = $"HDMUAHANG-{Nanoid.Generate(Nanoid.Alphabets.LowercaseLettersAndDigits, 5)}";
            Status = StatusEnum.Draft;
        }

        public virtual Tax? Tax { get; set; }
        public virtual ICollection<ExportForm> StockExportReports { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<CustomerBuyReceiptDetail> Details { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using NanoidDotNet;
using WareHouseManagement.Model.Entity.Customer_Entity;
using WareHouseManagement.Model.Form;

namespace WareHouseManagement.Model.Receipt
{
    public class CustomerBuyReceipt:ReceiptGeneric
    {
        public CustomerBuyReceipt()
        {
            Id = $"HDMUAHANG-{Nanoid.Generate(Nanoid.Alphabets.LowercaseLettersAndDigits, 5)}";
        }
        public DateTime DateOrder { get; set; }

        public virtual StockExportForm? StockExportReport { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<CustomerBuyReceiptDetail> Details { get; set; }
    }
}

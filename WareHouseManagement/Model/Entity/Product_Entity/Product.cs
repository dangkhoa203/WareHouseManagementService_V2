using Microsoft.EntityFrameworkCore;
using NanoidDotNet;
using WareHouseManagement.Model.Entity.Vendor_EntiTy;
using WareHouseManagement.Model.Receipt;


namespace WareHouseManagement.Model.Entity.Product_Entity
{
    public class Product : EntityGeneric
    {
        public  int PricePerUnit { get; set; }
        public required string MeasureUnit { get; set; }
        public Product() : base()
        {
            Id = $"SP-{Nanoid.Generate(Nanoid.Alphabets.LowercaseLettersAndDigits, 5)}";
        }
        public virtual ProductType? ProductGroup { get; set; }
        public virtual ICollection<VendorReplenishReceiptDetail>? VendorReplenishReceiptDetails { get; set; }
        public virtual ICollection<CustomerBuyReceiptDetail>? CustomerBuyReceiptDetails { get; set; }
        public virtual Stock? Stocks { get; set; }
    }
}

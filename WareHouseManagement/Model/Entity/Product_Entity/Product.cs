using Microsoft.EntityFrameworkCore;
using NanoidDotNet;
using WareHouseManagement.Model.Entity.Generic;
using WareHouseManagement.Model.Entity.Vendor_EntiTy;
using WareHouseManagement.Model.Entity.Warehouse_Entity;
using WareHouseManagement.Model.Form;
using WareHouseManagement.Model.Receipt;


namespace WareHouseManagement.Model.Entity.Product_Entity {
    public class Product : EntityGeneric
    {
        public required string Name { get; set; }
        public required float PricePerUnit { get; set; }
        public required string MeasureUnit { get; set; }
        public Product() : base()
        {
            Id = $"SP-{Nanoid.Generate(Nanoid.Alphabets.LowercaseLettersAndDigits, 5)}";
        }
        public virtual ProductType? ProductType { get; set; }
        public virtual ICollection<VendorReplenishReceiptDetail>? VendorReplenishReceiptDetails { get; set; }
        public virtual ICollection<CustomerBuyReceiptDetail>? CustomerBuyReceiptDetails { get; set; }
        public virtual ICollection<ImportFormDetail>? ImportDetails { get; set; }
        public virtual ICollection<ExportFormDetail>? ExportDetails { get; set; }
        public virtual ICollection<Stock>? Stocks { get; set; }
    }
}

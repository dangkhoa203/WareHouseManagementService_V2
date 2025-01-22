using Microsoft.EntityFrameworkCore;
using NanoidDotNet;
using System.ComponentModel.DataAnnotations;
using WareHouseManagement.Model.Entity.Product_Entity;
using WareHouseManagement.Model.Entity.Vendor_Entity;
using WareHouseManagement.Model.Receipt;

namespace WareHouseManagement.Model.Entity.Vendor_EntiTy
{
    public class Vendor : EntityGeneric
    {
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Vendor() : base()
        {
            Id = $"NCC-{Nanoid.Generate(Nanoid.Alphabets.LowercaseLettersAndDigits, 5)}";
        }
        public virtual ICollection<VendorReplenishReceipt>? VendorReplenishReceipts {  get; set; } 
        public virtual VendorGroup? VendorGroup { get; set; }
    }
}

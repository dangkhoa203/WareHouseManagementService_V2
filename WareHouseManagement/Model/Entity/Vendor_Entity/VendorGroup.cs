using Microsoft.EntityFrameworkCore;
using NanoidDotNet;
using WareHouseManagement.Model.Entity.Generic;
using WareHouseManagement.Model.Entity.Vendor_EntiTy;

namespace WareHouseManagement.Model.Entity.Vendor_Entity {
    public class VendorGroup:EntityGeneric
    {
        public required string Name { get; set; }
        public string Description { get; set; }
        public VendorGroup() : base()
        {
            Id = $"NHOMNCC-{Nanoid.Generate(Nanoid.Alphabets.LowercaseLettersAndDigits, 5)}";
        }
        public virtual ICollection<Vendor>? Vendors { get; set; }
    }
}

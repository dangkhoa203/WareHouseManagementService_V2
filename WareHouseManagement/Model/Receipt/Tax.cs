using System.ComponentModel.DataAnnotations;
using NanoidDotNet;
using WareHouseManagement.Model.Entity.Generic;

namespace WareHouseManagement.Model.Receipt {
    public class Tax:EntityGeneric {
        public string Name { get; set; }
        public string Description { get; set; }
        public float Percent {  get; set; }
        public Tax():base() {
            Id= $"THUE-{Nanoid.Generate(Nanoid.Alphabets.LowercaseLettersAndDigits, 5)}";
        }
        public virtual ICollection<CustomerBuyReceipt>? CustomerReceipts { get; set; }
        public virtual ICollection<VendorReplenishReceipt>? VendorReceipts { get; set; }
    }
}

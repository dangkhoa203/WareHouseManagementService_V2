using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using NanoidDotNet;
using System.ComponentModel.DataAnnotations;
using WareHouseManagement.Model.Receipt;

namespace WareHouseManagement.Model.Entity.Customer_Entity
{
    public class Customer:EntityGeneric
    {

        public required string Address { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public Customer() : base()
        {
            Id = $"KH-{Nanoid.Generate(Nanoid.Alphabets.LowercaseLettersAndDigits, 5)}";
        }
        public virtual CustomerGroup? CustomerGroup { get; set; }

        public virtual ICollection<CustomerBuyReceipt>? CustomerBuyReceipts { get; set; }

    }
}

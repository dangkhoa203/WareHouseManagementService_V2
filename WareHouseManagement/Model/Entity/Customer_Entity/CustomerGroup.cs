using Microsoft.EntityFrameworkCore;
using NanoidDotNet;
using System.ComponentModel.DataAnnotations;

namespace WareHouseManagement.Model.Entity.Customer_Entity
{
    public class CustomerGroup:EntityGeneric
    {
 
        public string Description { get; set; }
        public CustomerGroup() : base()
        {
            Id = $"NHOMKH-{Nanoid.Generate(Nanoid.Alphabets.LowercaseLettersAndDigits, 5)}";
        }
        public virtual ICollection<Customer>? Customers { get; set; }


    }
}

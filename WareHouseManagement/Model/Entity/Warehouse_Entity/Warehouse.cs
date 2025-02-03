using System.ComponentModel.DataAnnotations;
using NanoidDotNet;
using WareHouseManagement.Model.Form;

namespace WareHouseManagement.Model.Entity.Warehouse_Entity
{
    public class Warehouse
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual Stock? Stocks { get; set; }
        public virtual ICollection<ImportFormDetail>? ImportDetails { get; set; }
        public Warehouse() {
            Id = $"NHAKHO-{Nanoid.Generate(Nanoid.Alphabets.LowercaseLettersAndDigits, 5)}";
            CreatedDate = DateTime.Now;
        }
    }
}

using System.ComponentModel.DataAnnotations;
using NanoidDotNet;
using WareHouseManagement.Model.Form;

namespace WareHouseManagement.Model.Entity.Warehouse_Entity {
    public class Warehouse : EntityGeneric {

        public string Address { get; set; }
        public string City { get; set; }
        public virtual ICollection<Stock>? Stocks { get; set; }
        public virtual ICollection<ImportFormDetail>? ImportDetails { get; set; }
        public virtual ICollection<ExportFormDetail>? ExportDetails { get; set; }
        public Warehouse() {
            Id = $"NHAKHO-{Nanoid.Generate(Nanoid.Alphabets.LowercaseLettersAndDigits, 5)}";
        }
    }
}

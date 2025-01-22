using System.ComponentModel.DataAnnotations;
using WareHouseManagement.Model.Entity;

namespace WareHouseManagement.Model.Form
{
    public class FormGeneric
    {
        [Key]
        public string Id { get; set; }
        public DateTime CreatedDate { get; set; }
       
        public FormGeneric()
        {
            CreatedDate = DateTime.Now;
        }
        public virtual ServiceRegistered ServiceRegisteredFrom { get; set; }
    }
}

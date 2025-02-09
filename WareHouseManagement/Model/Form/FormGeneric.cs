using System.ComponentModel.DataAnnotations;
using WareHouseManagement.Model.Entity;

namespace WareHouseManagement.Model.Form
{
    public class FormGeneric
    {
        [Key]
        public string Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public FormGeneric()
        {
            CreatedDate = DateTime.Now;
            IsDeleted = false;
            DeletedAt = null;
        }
        public virtual ServiceRegistered ServiceRegisteredFrom { get; set; }
    }
}

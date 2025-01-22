using System.ComponentModel.DataAnnotations;

namespace WareHouseManagement.Model.Entity
{
    public abstract class EntityGeneric
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public EntityGeneric()
        {
            CreatedDate = DateTime.Now;
            IsDeleted = false;
            DeletedAt = null;
        }
        public virtual ServiceRegistered ServiceRegisteredFrom { get; set; }
    }
}

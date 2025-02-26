using System.ComponentModel.DataAnnotations;

namespace WareHouseManagement.Model.Entity.Generic {
    public abstract class EntityGeneric {
        [Key]
        public string Id { get; set; }
        public required string ServiceId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public EntityGeneric() {
            CreatedDate = DateTime.Now;
            IsDeleted = false;
            DeletedAt = null;
        }
    }
}

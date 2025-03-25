using System.ComponentModel.DataAnnotations;
using WareHouseManagement.Model.Entity.Product_Entity;

namespace WareHouseManagement.Model.Entity.Warehouse_Entity {
    public class Stock {
        public  string ProductId { get; set; }
        public Product? ProductNav { get; set; }
        public  string WarehouseId { get; set; }
        public Warehouse? WarehouseNav { get; set; }
        public required string ServiceId { get; set; }
        public required int Quantity { get; set; }

    }
}

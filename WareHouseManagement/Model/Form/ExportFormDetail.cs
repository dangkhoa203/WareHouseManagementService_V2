using WareHouseManagement.Model.Entity.Product_Entity;
using WareHouseManagement.Model.Entity.Warehouse_Entity;

namespace WareHouseManagement.Model.Form {
    public class ExportFormDetail {
        public string ProductId { get; set; }
        public string FormId { get; set; }
        public string WarehouseId { get; set; }
        public required int Quantity { get; set; }
        public virtual Product ProductNav { get; set; }
        public virtual StockExportForm FormNav { get; set; }
        public virtual Warehouse WarehouseNav { get; set; }
    }
}

﻿using System.ComponentModel.DataAnnotations;
using WareHouseManagement.Model.Entity.Product_Entity;

namespace WareHouseManagement.Model.Receipt
{
    public class VendorReplenishReceiptDetail
    {
        public string ProductId { get; set; }
        public string ReceiptId { get; set; }
        public int Quantity { get; set; }
        public int PriceOfOne { get; set; }
        public int TotalPrice { get; set; }
        public virtual Product ProductNav { get; set; }
        public virtual VendorReplenishReceipt ReceiptNav { get; set; }
     
    }
}

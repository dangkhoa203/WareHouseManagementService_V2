using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using WareHouseManagement.Model.Entity;
using WareHouseManagement.Model.Entity.Customer_Entity;
using WareHouseManagement.Model.Entity.Product_Entity;
using WareHouseManagement.Model.Entity.Vendor_Entity;
using WareHouseManagement.Model.Entity.Vendor_EntiTy;
using WareHouseManagement.Model.Entity.Warehouse_Entity;
using WareHouseManagement.Model.Form;
using WareHouseManagement.Model.Receipt;
namespace WareHouseManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<Account>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option) : base(option)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Account>(entity =>
            entity.HasOne(e => e.ServiceRegistered)
                  .WithMany(s => s.Accounts)
                  .HasForeignKey(e => e.ServiceId)
            );
            builder.Entity<ProductType>(entity =>
            {
                entity.HasMany(t => t.Products)
                      .WithOne(p => p.ProductType)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });
            builder.Entity<CustomerGroup>(entity =>
            {
                entity.HasMany(t => t.Customers)
                      .WithOne(p => p.CustomerGroup)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });
            builder.Entity<VendorGroup>(entity =>
            {
                entity.HasMany(t => t.Vendors)
                      .WithOne(p => p.VendorGroup)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });
            builder.Entity<VendorReplenishReceiptDetail>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.ReceiptId });

                entity.HasOne(d => d.ProductNav)
                    .WithMany(p => p.VendorReplenishReceiptDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_replenishdetail_product");

                entity.HasOne(d => d.ReceiptNav)
                    .WithMany(p => p.Details)
                    .HasForeignKey(d => d.ReceiptId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_replenishdetail_receipt");
            });
            builder.Entity<CustomerBuyReceiptDetail>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.ReceiptId });

                entity.HasOne(d => d.ProductNav)
                    .WithMany(p => p.CustomerBuyReceiptDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_orderdetail_product");

                entity.HasOne(d => d.ReceiptNav)
                    .WithMany(p => p.Details)
                    .HasForeignKey(d => d.ReceiptId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_orderdetail_receipt");
            });
            builder.Entity<Stock>(entity => {
                entity.HasKey(e => new { e.ProductId, e.WarehouseId });

                entity.HasOne(s => s.ProductNav)
                   .WithMany(p => p.Stocks)
                   .HasForeignKey(s => s.ProductId)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_stock_product");

                entity.HasOne(s => s.WarehouseNav)
                   .WithMany(p => p.Stocks)
                   .HasForeignKey(s => s.WarehouseId)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_warehouse_receipt");
            });
           

            builder.Entity<StockExportForm>()
                   .HasOne(f => f.Receipt)
                   .WithOne(r => r.StockExportReport)
                   .HasForeignKey<StockExportForm>(f => f.ReceiptId)
                   .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<StockImportForm>()
                   .HasOne(f => f.Receipt)
                   .WithOne(r => r.StockImportReport)
                   .HasForeignKey<StockImportForm>(f => f.ReceiptId)
                   .OnDelete(DeleteBehavior.ClientSetNull);
            builder.Entity<ImportFormDetail>(entity => {
                entity.HasKey(e => new { e.ProductId, e.FormId,e.WarehouseId });

                entity.HasOne(d => d.ProductNav)
                    .WithMany(p => p.ImportDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_importdetail_product");

                entity.HasOne(d => d.FormNav)
                    .WithMany(p => p.Details)
                    .HasForeignKey(d => d.FormId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_importdetail_form");
                entity.HasOne(d => d.WarehouseNav)
                   .WithMany(p => p.ImportDetails)
                   .HasForeignKey(d => d.WarehouseId)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("FK_importdetail_warehouse");
            });
            builder.Entity<ImportFormDetail>(entity => {
                entity.HasKey(e => new { e.ProductId, e.FormId, e.WarehouseId });

                entity.HasOne(d => d.ProductNav)
                    .WithMany(p => p.ImportDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_exportdetail_product");

                entity.HasOne(d => d.FormNav)
                    .WithMany(p => p.Details)
                    .HasForeignKey(d => d.FormId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_exportdetail_form");
                entity.HasOne(d => d.WarehouseNav)
                   .WithMany(p => p.ImportDetails)
                   .HasForeignKey(d => d.WarehouseId)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("FK_exportdetail_warehouse");
            });
            base.OnModelCreating(builder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-N0V3KE1\SQLEXPRESS;Database=Dev;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
            base.OnConfiguring(optionsBuilder);
        }
        public virtual DbSet<ServiceRegistered> ServiceRegistereds { get; set; }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerGroup> CustomerGroups { get; set; }
        public virtual DbSet<CustomerBuyReceipt> CustomerBuyReceipts { get; set; }
        public virtual DbSet<CustomerBuyReceiptDetail> CustomerBuyReceiptDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductType> ProductTypes { get; set; }

        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<VendorGroup> VendorGroups { get; set; }
        public virtual DbSet<VendorReplenishReceipt> VendorReplenishReceipts { get; set; }
        public virtual DbSet<VendorReplenishReceiptDetail> VendorReplenishReceiptDetails { get; set; }
        public virtual DbSet<Warehouse> Warehouses { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<ExportFormDetail> ExportDetails { get; set; }
        public virtual DbSet<StockExportForm> StockExportForms { get; set; }
        public virtual DbSet<ImportFormDetail> ImportDetails { get; set; }
        public virtual DbSet<StockImportForm> StockImportForms { get; set; }


    }
}

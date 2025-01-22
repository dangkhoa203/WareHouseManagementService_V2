﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WareHouseManagement.Data;

#nullable disable

namespace WareHouseManagement.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250122014117_Intial")]
    partial class Intial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.Account", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ServiceId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex("ServiceId");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.Customer_Entity.Customer", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CustomerGroupId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ServiceRegisteredFromId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("CustomerGroupId");

                    b.HasIndex("ServiceRegisteredFromId");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.Customer_Entity.CustomerGroup", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ServiceRegisteredFromId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ServiceRegisteredFromId");

                    b.ToTable("CustomerGroups");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.Product_Entity.Product", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("MeasureUnit")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PricePerUnit")
                        .HasColumnType("int");

                    b.Property<string>("ProductGroupId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ServiceRegisteredFromId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ProductGroupId");

                    b.HasIndex("ServiceRegisteredFromId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.Product_Entity.ProductType", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ServiceRegisteredFromId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ServiceRegisteredFromId");

                    b.ToTable("ProductTypes");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.ServiceRegistered", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.ToTable("ServiceRegistereds");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.Stock", b =>
                {
                    b.Property<string>("ProductId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("ServiceRegisteredFromId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ProductId");

                    b.HasIndex("ServiceRegisteredFromId");

                    b.ToTable("Stocks");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.Vendor_EntiTy.Vendor", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ServiceRegisteredFromId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("VendorGroupId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ServiceRegisteredFromId");

                    b.HasIndex("VendorGroupId");

                    b.ToTable("Vendors");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.Vendor_Entity.VendorGroup", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ServiceRegisteredFromId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ServiceRegisteredFromId");

                    b.ToTable("VendorGroups");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Form.StockExportForm", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ReceiptId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ServiceRegisteredFromId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ReceiptId")
                        .IsUnique();

                    b.HasIndex("ServiceRegisteredFromId");

                    b.ToTable("StockExportReports");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Form.StockImportForm", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ReceiptId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ServiceRegisteredFromId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ReceiptId")
                        .IsUnique();

                    b.HasIndex("ServiceRegisteredFromId");

                    b.ToTable("StockImportReports");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Receipt.CustomerBuyReceipt", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CustomerId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateOrder")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("ServiceRegisteredFromId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("ServiceRegisteredFromId");

                    b.ToTable("CustomerBuyReceipts");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Receipt.CustomerBuyReceiptDetail", b =>
                {
                    b.Property<string>("ProductId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ReceiptId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("ProductId", "ReceiptId");

                    b.HasIndex("ReceiptId");

                    b.ToTable("CustomerBuyReceiptDetails");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Receipt.VendorReplenishReceipt", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateOrder")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("ServiceRegisteredFromId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("VendorId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ServiceRegisteredFromId");

                    b.HasIndex("VendorId");

                    b.ToTable("VendorReplenishReceipts");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Receipt.VendorReplenishReceiptDetail", b =>
                {
                    b.Property<string>("ProductId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ReceiptId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("ProductId", "ReceiptId");

                    b.HasIndex("ReceiptId");

                    b.ToTable("VendorReplenishReceiptDetails");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("WareHouseManagement.Model.Entity.Account", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("WareHouseManagement.Model.Entity.Account", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WareHouseManagement.Model.Entity.Account", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("WareHouseManagement.Model.Entity.Account", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.Account", b =>
                {
                    b.HasOne("WareHouseManagement.Model.Entity.ServiceRegistered", "ServiceRegistered")
                        .WithMany("Accounts")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServiceRegistered");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.Customer_Entity.Customer", b =>
                {
                    b.HasOne("WareHouseManagement.Model.Entity.Customer_Entity.CustomerGroup", "CustomerGroup")
                        .WithMany("Customers")
                        .HasForeignKey("CustomerGroupId");

                    b.HasOne("WareHouseManagement.Model.Entity.ServiceRegistered", "ServiceRegisteredFrom")
                        .WithMany()
                        .HasForeignKey("ServiceRegisteredFromId");

                    b.Navigation("CustomerGroup");

                    b.Navigation("ServiceRegisteredFrom");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.Customer_Entity.CustomerGroup", b =>
                {
                    b.HasOne("WareHouseManagement.Model.Entity.ServiceRegistered", "ServiceRegisteredFrom")
                        .WithMany()
                        .HasForeignKey("ServiceRegisteredFromId");

                    b.Navigation("ServiceRegisteredFrom");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.Product_Entity.Product", b =>
                {
                    b.HasOne("WareHouseManagement.Model.Entity.Product_Entity.ProductType", "ProductGroup")
                        .WithMany("Products")
                        .HasForeignKey("ProductGroupId");

                    b.HasOne("WareHouseManagement.Model.Entity.ServiceRegistered", "ServiceRegisteredFrom")
                        .WithMany()
                        .HasForeignKey("ServiceRegisteredFromId");

                    b.Navigation("ProductGroup");

                    b.Navigation("ServiceRegisteredFrom");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.Product_Entity.ProductType", b =>
                {
                    b.HasOne("WareHouseManagement.Model.Entity.ServiceRegistered", "ServiceRegisteredFrom")
                        .WithMany()
                        .HasForeignKey("ServiceRegisteredFromId");

                    b.Navigation("ServiceRegisteredFrom");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.Stock", b =>
                {
                    b.HasOne("WareHouseManagement.Model.Entity.Product_Entity.Product", "ProductNav")
                        .WithOne("Stocks")
                        .HasForeignKey("WareHouseManagement.Model.Entity.Stock", "ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WareHouseManagement.Model.Entity.ServiceRegistered", "ServiceRegisteredFrom")
                        .WithMany()
                        .HasForeignKey("ServiceRegisteredFromId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProductNav");

                    b.Navigation("ServiceRegisteredFrom");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.Vendor_EntiTy.Vendor", b =>
                {
                    b.HasOne("WareHouseManagement.Model.Entity.ServiceRegistered", "ServiceRegisteredFrom")
                        .WithMany()
                        .HasForeignKey("ServiceRegisteredFromId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WareHouseManagement.Model.Entity.Vendor_Entity.VendorGroup", "VendorGroup")
                        .WithMany("Vendors")
                        .HasForeignKey("VendorGroupId");

                    b.Navigation("ServiceRegisteredFrom");

                    b.Navigation("VendorGroup");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.Vendor_Entity.VendorGroup", b =>
                {
                    b.HasOne("WareHouseManagement.Model.Entity.ServiceRegistered", "ServiceRegisteredFrom")
                        .WithMany()
                        .HasForeignKey("ServiceRegisteredFromId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServiceRegisteredFrom");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Form.StockExportForm", b =>
                {
                    b.HasOne("WareHouseManagement.Model.Receipt.CustomerBuyReceipt", "Receipt")
                        .WithOne("StockExportReport")
                        .HasForeignKey("WareHouseManagement.Model.Form.StockExportForm", "ReceiptId")
                        .IsRequired();

                    b.HasOne("WareHouseManagement.Model.Entity.ServiceRegistered", "ServiceRegisteredFrom")
                        .WithMany()
                        .HasForeignKey("ServiceRegisteredFromId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Receipt");

                    b.Navigation("ServiceRegisteredFrom");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Form.StockImportForm", b =>
                {
                    b.HasOne("WareHouseManagement.Model.Receipt.VendorReplenishReceipt", "Receipt")
                        .WithOne("StockImportReport")
                        .HasForeignKey("WareHouseManagement.Model.Form.StockImportForm", "ReceiptId")
                        .IsRequired();

                    b.HasOne("WareHouseManagement.Model.Entity.ServiceRegistered", "ServiceRegisteredFrom")
                        .WithMany()
                        .HasForeignKey("ServiceRegisteredFromId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Receipt");

                    b.Navigation("ServiceRegisteredFrom");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Receipt.CustomerBuyReceipt", b =>
                {
                    b.HasOne("WareHouseManagement.Model.Entity.Customer_Entity.Customer", "Customer")
                        .WithMany("CustomerBuyReceipts")
                        .HasForeignKey("CustomerId");

                    b.HasOne("WareHouseManagement.Model.Entity.ServiceRegistered", "ServiceRegisteredFrom")
                        .WithMany()
                        .HasForeignKey("ServiceRegisteredFromId");

                    b.Navigation("Customer");

                    b.Navigation("ServiceRegisteredFrom");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Receipt.CustomerBuyReceiptDetail", b =>
                {
                    b.HasOne("WareHouseManagement.Model.Entity.Product_Entity.Product", "ProductNav")
                        .WithMany("CustomerBuyReceiptDetails")
                        .HasForeignKey("ProductId")
                        .IsRequired()
                        .HasConstraintName("FK_orderdetail_product");

                    b.HasOne("WareHouseManagement.Model.Receipt.CustomerBuyReceipt", "ReceiptNav")
                        .WithMany("Details")
                        .HasForeignKey("ReceiptId")
                        .IsRequired()
                        .HasConstraintName("FK_orderdetail_receipt");

                    b.Navigation("ProductNav");

                    b.Navigation("ReceiptNav");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Receipt.VendorReplenishReceipt", b =>
                {
                    b.HasOne("WareHouseManagement.Model.Entity.ServiceRegistered", "ServiceRegisteredFrom")
                        .WithMany()
                        .HasForeignKey("ServiceRegisteredFromId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WareHouseManagement.Model.Entity.Vendor_EntiTy.Vendor", "Vendor")
                        .WithMany("VendorReplenishReceipts")
                        .HasForeignKey("VendorId");

                    b.Navigation("ServiceRegisteredFrom");

                    b.Navigation("Vendor");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Receipt.VendorReplenishReceiptDetail", b =>
                {
                    b.HasOne("WareHouseManagement.Model.Entity.Product_Entity.Product", "ProductNav")
                        .WithMany("VendorReplenishReceiptDetails")
                        .HasForeignKey("ProductId")
                        .IsRequired()
                        .HasConstraintName("FK_replenishdetail_product");

                    b.HasOne("WareHouseManagement.Model.Receipt.VendorReplenishReceipt", "ReceiptNav")
                        .WithMany("Details")
                        .HasForeignKey("ReceiptId")
                        .IsRequired()
                        .HasConstraintName("FK_replenishdetail_receipt");

                    b.Navigation("ProductNav");

                    b.Navigation("ReceiptNav");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.Customer_Entity.Customer", b =>
                {
                    b.Navigation("CustomerBuyReceipts");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.Customer_Entity.CustomerGroup", b =>
                {
                    b.Navigation("Customers");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.Product_Entity.Product", b =>
                {
                    b.Navigation("CustomerBuyReceiptDetails");

                    b.Navigation("Stocks");

                    b.Navigation("VendorReplenishReceiptDetails");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.Product_Entity.ProductType", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.ServiceRegistered", b =>
                {
                    b.Navigation("Accounts");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.Vendor_EntiTy.Vendor", b =>
                {
                    b.Navigation("VendorReplenishReceipts");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Entity.Vendor_Entity.VendorGroup", b =>
                {
                    b.Navigation("Vendors");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Receipt.CustomerBuyReceipt", b =>
                {
                    b.Navigation("Details");

                    b.Navigation("StockExportReport");
                });

            modelBuilder.Entity("WareHouseManagement.Model.Receipt.VendorReplenishReceipt", b =>
                {
                    b.Navigation("Details");

                    b.Navigation("StockImportReport");
                });
#pragma warning restore 612, 618
        }
    }
}

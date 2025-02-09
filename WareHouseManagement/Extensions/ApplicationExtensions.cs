using WareHouseManagement.Feature.Accounts;
using WareHouseManagement.Feature.Accounts.ChangeEmail;
using WareHouseManagement.Feature.Accounts.ChangeFullName;
using WareHouseManagement.Feature.Accounts.ChangePassword;
using WareHouseManagement.Feature.Accounts.Email;
using WareHouseManagement.Feature.Accounts.ResetPassword;
using WareHouseManagement.Feature.CustomerBuyReceipts;
using WareHouseManagement.Feature.CustomerGroups;
using WareHouseManagement.Feature.Customers;
using WareHouseManagement.Feature.ImportForm;
using WareHouseManagement.Feature.Products;
using WareHouseManagement.Feature.ProductTypes;
using WareHouseManagement.Feature.Stocks;
using WareHouseManagement.Feature.VendorGroups;
using WareHouseManagement.Feature.VendorReplenishReceipts;
using WareHouseManagement.Feature.Vendors;
using WareHouseManagement.Feature.Warehouses;
using WareHouseManagement.Model.Entity.Vendor_Entity;

namespace WareHouseManagement.Extensions {
    public static class ApplicationExtensions {

        private static void AddAccountService(this WebApplication app) {
            Login.MapEndpoint(app);
            Register.MapEndpoint(app);
            GetAccount.MapEndpoint(app);
            LogOut.MapEndpoint(app);
            ConfirmAccount.MapEndpoint(app);
            SendResetPasswordRequest.MapEndpoint(app);
            CheckAccountForReset.MapEndpoint(app);
            ResetPassword.MapEndpoint(app);
            SendChangePasswordRequest.MapEndpoint(app);
            ChangePassword.MapEndpoint(app);
            SendChangeEmailRequest.MapEndpoint(app);
            ChangeEmail.MapEndpoint(app);
            ChangeFullname.MapEndpoint(app);
        }
        private static void AddCustomerService(this WebApplication app) {
            AddCustomer.MapEndpoint(app);
            RemoveCustomer.MapEndpoint(app);
            UpdateCustomer.MapEndpoint(app);
            GetCustomer.MapEndpoint(app);
            GetCustomers.MapEndpoint(app);

            AddCustomerGroup.MapEndpoint(app);
            RemoveCustomerGroup.MapEndpoint(app);
            UpdateCustomerGroup.MapEndpoint(app);
            GetCustomerGroups.MapEndpoint(app);
            GetCustomerGroup.MapEndpoint(app);
        }
        private static void AddVendorService(this WebApplication app) {
            AddVendorGroup.MapEndpoint(app);
            RemoveVendorGroup.MapEndpoint(app);
            UpdateVendorGroup.MapEndpoint(app);
            GetVendorGroups.MapEndpoint(app);
            GetVendorGroup.MapEndpoint(app);

            AddVendor.MapEndpoint(app);
            RemoveVendor.MapEndpoint(app);
            UpdateVendor.MapEndpoint(app);
            GetVendors.MapEndpoint(app);
            GetVendor.MapEndpoint(app);
        }
        private static void AddProductService(this WebApplication app) {
            AddProductType.MapEndpoint(app);
            RemoveProductType.MapEndpoint(app);
            UpdateProductType.MapEndpoint(app);
            GetProductTypes.MapEndpoint(app);
            GetProductType.MapEndpoint(app);

            AddProduct.MapEndpoint(app);
            RemoveProduct.MapEndpoint(app);
            UpdateProduct.MapEndpoint(app);
            GetProducts.MapEndpoint(app);
            GetProduct.MapEndpoint(app);
        }
        private static void AddWarehouseSevice(this WebApplication app) {
            AddWarehouse.MapEndpoint(app);
            RemoveWarehouse.MapEndpoint(app);
            UpdateWarehouse.MapEndpoint(app);
            GetWarehouses.MapEndpoint(app);
        }
        private static void AddCustomerReceiptService(this WebApplication app) {
            AddCustomerReceipt.MapEndpoint(app);
            RemoveCustomerReceipt.MapEndpoint(app);
            GetCustomerReceipts.MapEndpoint(app);
            GetCustomerReceipt.MapEndpoint(app);
        }
        private static void AddVendorReceiptSevice(this WebApplication app) {
            AddVendorReceipt.MapEndpoint(app);
            RemoveVendorReceipt.MapEndpoint(app);
            GetVendorReceipts.MapEndpoint(app);
            GetVendorReceipt.MapEndpoint(app);
        }
        private static void AddStockService(this WebApplication app) {
            AddStocks.MapEndpoint(app);
            RemoveStock.MapEndpoint(app);
            UpdateStock.MapEndpoint(app);
            GetStocks.MapEndpoint(app);
        }
        private static void AddImportFormService(this WebApplication app) {
            AddImportForm.MapEndpoint(app);
            RemoveImportForm.MapEndpoint(app);
            GetImportForms.MapEndpoint(app);
            GetImportForm.MapEndpoint(app);
        }
        public static void AddAllEndPoint(this WebApplication app) {
            AddAccountService(app);
            AddCustomerService(app);
            AddVendorService(app);
            AddProductService(app);
            AddWarehouseSevice(app);
            AddCustomerReceiptService(app);
            AddVendorReceiptSevice(app);
            AddStockService(app);
            AddImportFormService(app);
        }
    }
}

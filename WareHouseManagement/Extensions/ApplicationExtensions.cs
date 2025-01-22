using WareHouseManagement.Feature.Accounts;
using WareHouseManagement.Feature.Accounts.ChangeEmail;
using WareHouseManagement.Feature.Accounts.ChangeFullName;
using WareHouseManagement.Feature.Accounts.ChangePassword;
using WareHouseManagement.Feature.Accounts.Email;
using WareHouseManagement.Feature.Accounts.ResetPassword;
using WareHouseManagement.Feature.CustomerGroups;
using WareHouseManagement.Feature.Customers;
using WareHouseManagement.Feature.ProductTypes;

namespace WareHouseManagement.Extensions
{
    public static class ApplicationExtensions
    {

        private static void AddAccountService(this WebApplication app)
        {
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
        private static void AddCustomerService(this WebApplication app)
        {
            AddCustomer.MapEndpoint(app);
            RemoveCustomer.MapEndpoint(app);
            UpdateCustomer.MapEndpoint(app);
            GetCustomer.MapEndpoint(app);
            GetCustomers.MapEndpoint(app);

            AddCustomerGroup.MapEndpoint(app);
            RemoveCustomerGroup.MapEndpoint(app);
            UpdateCustomerGroup.MapEndpoint(app);
            GetCustomerGroups.MapEndpoint(app);
            GetCutomerGroup.MapEndpoint(app);
        }
        private static void AddProductService(this WebApplication app)
        {
            AddProductType.MapEndpoint(app);
            RemoveProductType.MapEndpoint(app);
            UpdateProductType.MapEndpoint(app);
            GetProductTypes.MapEndpoint(app);
            GetProductType.MapEndpoint(app);
        }
        public static void AddAllEndPoint(this WebApplication app)
        {
            AddAccountService(app);
            AddCustomerService(app);
            AddProductService(app);
        }
    }
}

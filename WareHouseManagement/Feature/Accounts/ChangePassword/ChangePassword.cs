using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Account;

namespace WareHouseManagement.Feature.Accounts.ChangePassword {
    public class ChangePassword : IEndpoint {
        public record Response(bool Success, string ErrorMessage);
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPut("api/Account/PasswordChange/{userId}/{newPassword}/{code}", Handler).WithTags("Account");
        }
        private static async Task<IResult> Handler(string userId, string newPassword, string code, UserManager<Account> userManager) {
            try {
                Account User = await userManager.FindByIdAsync(Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(userId)));
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                newPassword = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(newPassword));

                var Result = await userManager.ResetPasswordAsync(User, code, newPassword);
                if (!Result.Succeeded) {
                    return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!"));
                }

                return Results.Ok(new Response(true, ""));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra!"));
            }
        }
    }
}

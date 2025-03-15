using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Account;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Accounts {
    public class ConfirmAccount : IEndpoint {
        public record Response(bool Success, string ErrorMessage);
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPut("/api/Account/ConfirmAccount/{userName}/{code}", Handler).WithTags("Account");
        }
        private static async Task<IResult> Handler([FromRoute] string userName, [FromRoute] string code, UserManager<Account> userManager) {
            try {
                Account User = await userManager.FindByNameAsync(Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(userName)));
                if (User == null) {
                    return Results.NotFound(new Response(false, "Người dùng không tìm thấy"));
                }

                if (User.EmailConfirmed) {
                    return Results.BadRequest(new Response(false, "Đã xác nhận tài khoản."));
                }

                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                var result = await userManager.ConfirmEmailAsync(User, code);
                if (!result.Succeeded) {
                    return Results.BadRequest(new Response(false, "Lỗi đã xảy ra"));
                }

                await userManager.AddToRoleAsync(User, Permission.Admin);
                return Results.Ok(new Response(true, ""));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra"));
            }

        }

    }
}

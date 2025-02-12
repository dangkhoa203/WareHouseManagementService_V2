using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity;

namespace WareHouseManagement.Feature.Accounts
{
    public class ConfirmAccount : IEndpoint
    {
        public record Response(bool success,string errorMessage);
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/Account/ConfirmAccount/{userName}/{code}", Handler).WithTags("Account");
        }
        private static async Task<IResult> Handler(
            [FromRoute] string userName,
            [FromRoute] string code,
            UserManager<Account> userManager)
        {
            Account user = await userManager.FindByNameAsync(Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(userName)));
            if (user != null)
            {
                if (user.EmailConfirmed)
                {
                    return Results.BadRequest(new Response(false,"Đã xác nhận tài khoản."));
                }
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                var result = await userManager.ConfirmEmailAsync(user, code);
                if (!result.Succeeded)
                {
                    return Results.BadRequest(new Response(false,"Lỗi đã xảy ra"));
                }
                await userManager.AddToRoleAsync(user, Permission.Admin.ToString());
            }
            else
            {
                return Results.NotFound(new Response(false,"Người dùng không tìm thấy"));
            }
            return Results.Ok(new Response(true,""));
        }

    }
}

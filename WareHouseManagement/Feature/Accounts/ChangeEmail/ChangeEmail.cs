using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity;

namespace WareHouseManagement.Feature.Accounts.ChangeEmail
{
    public class ChangeEmail : IEndpoint
    {
        public record Response(bool success, string errorMessage);
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/Account/EmailChange/{userId}/{newEmail}/{code}", Handler).WithTags("Account");
        }
        private static async Task<IResult> Handler(string userId, string newEmail, string code, UserManager<Account> userManager)
        {
            Account user = await userManager.FindByIdAsync(Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(userId)));
            if (user != null)
            {
                newEmail = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(newEmail));
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                var result = userManager.ChangeEmailAsync(user, newEmail, code);
                if (!result.Result.Succeeded)
                {
                    return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!"));
                }
                return Results.BadRequest(new Response(true, ""));
            }
            return Results.BadRequest(new Response(false, "Không tìm ra người dùng!"));
        }
    }
}

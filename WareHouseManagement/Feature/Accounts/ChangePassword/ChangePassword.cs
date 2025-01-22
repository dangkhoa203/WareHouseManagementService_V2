using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity;

namespace WareHouseManagement.Feature.Accounts.ChangePassword
{
    public class ChangePassword : IEndpoint
    {
        public record Response(bool success,string errorMessage);
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/Account/PasswordChange/{userId}/{newPassword}/{code}", Handler).WithTags("Account");
        }
        private static async Task<IResult> Handler(string userId, string newPassword, string code, UserManager<Account> userManager)
        {
            Account user = await userManager.FindByIdAsync(Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(userId)));
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            newPassword = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(newPassword));
            var result = await userManager.ResetPasswordAsync(user, code, newPassword);
            if (!result.Succeeded)
            {
                return Results.BadRequest(new Response(false,"Lỗi đã xảy ra!"));
            }
            return Results.Ok(new Response(true,""));
        }
    }
}

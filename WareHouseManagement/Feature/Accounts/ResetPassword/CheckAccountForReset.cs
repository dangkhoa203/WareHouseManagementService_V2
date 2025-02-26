using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Account;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Accounts.ResetPassword {
    public class CheckAccountForReset : IEndpoint
    {
        public record Response(bool success);
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/Account/PasswordReset/ResetValidation/{userid}/", Handler).WithTags("Account");
        }
        private static async Task<IResult> Handler([FromRoute] string userid, UserManager<Account> userManager)
        {
            Account user = await userManager.FindByIdAsync(Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(userid)));
            if (user != null && await userManager.IsInRoleAsync(user, Permission.Admin))
            {
                return Results.Ok(new Response(true));
            }
            return Results.Ok(new Response(false));
        }
    }
}

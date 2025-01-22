using Microsoft.AspNetCore.Identity;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity;

namespace WareHouseManagement.Feature.Accounts
{
    public class LogOut : IEndpoint
    {
        public record Response(bool success,string errorMessage);
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/Account/LogOut", Handler).RequireAuthorization().WithTags("Account");
        }
        private static async Task<IResult> Handler(SignInManager<Account> signInManager)
        {
            try
            {
                await signInManager.SignOutAsync();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new Response(false,"Lỗi đã xảy ra"));
            }
            return Results.Ok(new Response(true,""));
        }
    }
}

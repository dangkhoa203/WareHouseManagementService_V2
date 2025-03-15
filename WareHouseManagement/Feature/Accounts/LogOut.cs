using Microsoft.AspNetCore.Identity;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Account;

namespace WareHouseManagement.Feature.Accounts {
    public class LogOut : IEndpoint {
        public record Response(bool Success, string ErrorMessage);
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Account/LogOut", Handler).RequireAuthorization().WithTags("Account");
        }
        private static async Task<IResult> Handler(SignInManager<Account> signInManager) {
            try {
                await signInManager.SignOutAsync();
                return Results.Ok(new Response(true, ""));
            }
            catch (Exception ex) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra"));
            }
        }
    }
}

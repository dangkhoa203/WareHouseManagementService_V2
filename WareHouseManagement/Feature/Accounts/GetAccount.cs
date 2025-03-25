using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Account;
using WareHouseManagement.Model.Entity.Customer_Entity;

namespace WareHouseManagement.Feature.Accounts {
    public class GetAccount : IEndpoint {
        public record Response(string Username, string Userfullname, string Useremail, string Userid, bool Islogged);
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Account/", Handler).WithTags("Account");
        }
        private static async Task<IResult> Handler(UserManager<Account> userManager, SignInManager<Account> signInManager, ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                if (User.Identity.Name == null)
                    return Results.Ok(new Response("", "", "", "", false));

                Account Info = await userManager.FindByNameAsync(User.Identity.Name);
                await signInManager.RefreshSignInAsync(Info);

                return Results.Ok(new Response(Info.UserName, Info.FullName, Info.Email, Info.Id, true));
            }
            catch (Exception ex) {
                return Results.Ok(new Response("", "", "", "", false));
            }

        }
    }
}

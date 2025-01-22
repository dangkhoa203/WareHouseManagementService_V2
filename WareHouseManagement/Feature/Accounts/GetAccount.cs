using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity;
using WareHouseManagement.Model.Entity.Customer_Entity;

namespace WareHouseManagement.Feature.Accounts
{
    public class GetAccount:IEndpoint
    {
        public record Response(string username,string userfullname,string useremail,string userid,bool islogged);
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/Account/", Handler).WithTags("Account");
        }
        private static async Task<IResult> Handler(UserManager<Account> userManager,SignInManager<Account> signInManager,ApplicationDbContext context, ClaimsPrincipal user)
        {
            if (user.Identity.Name != null)
            {
                Account info = await userManager.FindByNameAsync(user.Identity.Name);
                await signInManager.RefreshSignInAsync(info);
                return Results.Ok(new Response(
                    info.UserName,
                    info.FullName,
                    info.Email,
                    info.Id,
                    true
                    ));
            }
            return Results.Ok(new Response("", "", "", "", false));
        }
    }
}

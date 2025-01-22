using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity;

namespace WareHouseManagement.Feature.Accounts.ChangeFullName
{
    public class ChangeFullname : IEndpoint
    {
        public record Request(string fullName);
        public record Response(bool success,string errorMessage,ValidationResult? result);
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(r => r.fullName).NotEmpty().WithMessage("Chưa nhập tên");
                RuleFor(r => r.fullName).MinimumLength(3).WithMessage("Phải nhập tối thiểu 3 ký tự");
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/Account/FullNameChange/", Handler).WithTags("Account");
        }
        private static async Task<IResult> Handler(Request request, UserManager<Account> userManager, ClaimsPrincipal user)
        {
            Account userDetail = await userManager.FindByNameAsync(user.Identity.Name);

            var validator = new Validator();
            var validateresult = await validator.ValidateAsync(request);
            if (!validateresult.IsValid)
                return Results.BadRequest(new Response(false, "", validateresult));

            userDetail.FullName =  request.fullName;
            var result = userManager.UpdateAsync(userDetail);
            if (!result.Result.Succeeded)
            {
                return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!", validateresult));
            }
            return Results.Ok(new Response(true, "", validateresult));
        } 
    }
}

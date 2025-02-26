using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Account;

namespace WareHouseManagement.Feature.Accounts {
    public class Login : IEndpoint
    {
        public record Request(string userName,string password,bool remember);
        public record Response(bool success, string errorMessage, ValidationResult? validateError);
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(r => r.userName).NotEmpty().WithMessage("Chưa nhập tên");
                RuleFor(r => r.password).NotEmpty().WithMessage("Chưa nhập mật khẩu");
                RuleFor(r=>r.password).MinimumLength(3).WithMessage("Phải nhập tối thiểu 3 ký tự");
                
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/Account/Login", Handler).WithTags("Account");
        }
        public static async Task<IResult> Handler(Request request, UserManager<Account> userManager,SignInManager<Account> signInManager, ClaimsPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                await signInManager.SignOutAsync();
            }
            var validator = new Validator();
            var validateresult = await validator.ValidateAsync(request);
            if (!validateresult.IsValid)
            {
                return Results.BadRequest(new Response(false,"Lỗi xảy ra", validateresult));
            }
            var loginuser = await userManager.FindByNameAsync(request.userName);
            if (loginuser != null)
            {
                var result = await signInManager.PasswordSignInAsync(loginuser, request.password, request.remember, false);
                if (result.Succeeded)
                {
                    return Results.Ok(new Response(true,"",null));
                }
            }
            return Results.BadRequest(new Response(false, "Hãy kiểm tra lại thông tin đăng nhập", null));
        }
    }
}

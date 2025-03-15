using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Account;

namespace WareHouseManagement.Feature.Accounts {
    public class Login : IEndpoint {
        public record Request(string UserName, string Password, bool Remember);
        public record Response(bool Success, string ErrorMessage, ValidationResult? ValidateError);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.UserName).NotEmpty().WithMessage("Chưa nhập tên");
                RuleFor(r => r.Password).NotEmpty().WithMessage("Chưa nhập mật khẩu");
                RuleFor(r => r.Password).MinimumLength(3).WithMessage("Phải nhập tối thiểu 3 ký tự");

            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Account/Login", Handler).WithTags("Account");
        }
        public static async Task<IResult> Handler(Request request, UserManager<Account> userManager, SignInManager<Account> signInManager, ClaimsPrincipal User) {
            try {
                if (User.Identity.IsAuthenticated) {
                    await signInManager.SignOutAsync();
                }

                var Validator = new Validator();
                var ValidateResult = await Validator.ValidateAsync(request);
                if (!ValidateResult.IsValid) {
                    return Results.BadRequest(new Response(false, "Lỗi xảy ra", ValidateResult));
                }

                var LoginUser = await userManager.FindByNameAsync(request.UserName);
                if (LoginUser != null) {
                    var result = await signInManager.PasswordSignInAsync(LoginUser, request.Password, request.Remember, false);
                    if (result.Succeeded) {
                        return Results.Ok(new Response(true, "", null));
                    }
                }

                return Results.BadRequest(new Response(false, "Hãy kiểm tra lại thông tin đăng nhập", ValidateResult));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra", null));
            }
        }
    }
}

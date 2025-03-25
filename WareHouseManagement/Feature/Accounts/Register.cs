using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Middleware;
using WareHouseManagement.Model.Entity.Account;

namespace WareHouseManagement.Feature.Accounts {
    public class Register : IEndpoint {
        public record Request(string FullName,string UserName, string Password, string ConfirmPassword, string Email);
        public record Response(bool Success, string ErrorMessage, ValidationResult? validateError);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.UserName).NotEmpty().WithMessage("Chưa nhập tên");
                RuleFor(r => r.Password).NotEmpty().WithMessage("Chưa nhập mật khẩu");
                RuleFor(r => r.Password).MinimumLength(3).WithMessage("Phải nhập tối thiểu 3 ký tự");
                RuleFor(r => r.Email).EmailAddress().WithMessage("Email chưa hợp lệ");
                RuleFor(r => r.ConfirmPassword).Equal(r => r.Password).WithMessage("Xác nhận mật khẩu chưa hợp lệ");
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Account/Register", Handler).WithTags("Account");
        }
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, UserManager<Account> userManager, SignInManager<Account> signInManager, ClaimsPrincipal User) {
            try {
                if (User.Identity.IsAuthenticated) {
                    return Results.BadRequest(new Response(false, "Đã đăng nhập", null));
                }

                var Validator = new Validator();
                var ValidateResult = await Validator.ValidateAsync(request);
                if (!ValidateResult.IsValid) {
                    return Results.BadRequest(new Response(false, "Lỗi xảy ra", ValidateResult));
                }

                if (await userManager.FindByEmailAsync(request.Email) != null) {
                    return Results.BadRequest(new Response(false, "Email đã có người đăng ký!", ValidateResult));
                }

                if (await userManager.FindByNameAsync(request.UserName) != null) {
                    return Results.BadRequest(new Response(false, "Tên đăng nhập đã có người đăng ký!", ValidateResult));
                }

                var ServiceRegistered = new ServiceRegistered();
                Account account = new() {
                    Email = request.Email,
                    UserName = request.UserName,
                    FullName = request.FullName,
                    isAdmin = true,
                    ServiceRegistered = ServiceRegistered,
                };
                var Result = await userManager.CreateAsync(account, request.Password);

                if (!Result.Succeeded) {
                    return Results.BadRequest(new Response(false, "Lỗi đã xảy ra", ValidateResult));
                }

                var CreatedUser = await userManager.FindByEmailAsync(request.Email);
                var Token = await userManager.GenerateEmailConfirmationTokenAsync(CreatedUser);
                Token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(Token));
                var ConfirmLink = $"https://localhost:7088/ConfirmEmail/{WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(CreatedUser.UserName))}/{Token}";
                var body = $"Xác nhận email tại <a href='{ConfirmLink}'>đây</a>";
                bool EmailResponse = await EmailSender.SendEmail(CreatedUser.Email, "Xác nhận Email tài khoản", "Xác nhận tài khoản bạn vừa mới đăng ký!", ConfirmLink, "Xác nhận");
                if (!EmailResponse) {
                    context.Users.Remove(CreatedUser);
                    await context.SaveChangesAsync();
                    return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!", ValidateResult));
                }
                CreatedUser.EmailConfirmed = true;
                await context.SaveChangesAsync();
                return Results.Ok(new Response(true, "", ValidateResult));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra!", null));
            }
        }
    }
}

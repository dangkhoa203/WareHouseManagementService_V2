using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Middleware;
using WareHouseManagement.Model.Entity.Account;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Accounts.ResetPassword {
    public class SendResetPasswordRequest : IEndpoint {
        public record Request(string Email);
        public record Response(bool Success, string ErrorMessage, ValidationResult? ValidateError);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.Email).EmailAddress().WithMessage("Email chưa hợp lệ");
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Account/PasswordReset/", Handler).WithTags("Account");
        }
        private static async Task<IResult> Handler(Request request, UserManager<Account> userManager) {
            try {
                var Validator = new Validator();
                var ValidateResult = await Validator.ValidateAsync(request);
                if (!ValidateResult.IsValid) {
                    return Results.BadRequest(new Response(false, "Lỗi xảy ra", ValidateResult));
                }

                Account User = await userManager.FindByEmailAsync(request.Email);
                if (User == null) {
                    return Results.NotFound(new Response(false, "Không có tài khoản với email này!", ValidateResult));
                }

                if (!await userManager.IsInRoleAsync(User, Permission.Admin)) {
                    return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!", ValidateResult));
                }

                var Token = await userManager.GeneratePasswordResetTokenAsync(User);
                Token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(Token));
                var ConfirmLink = $"https://localhost:7088/ResetPassword/{WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(User.Id))}/{Token}";

                bool EmailResponse = await EmailSender.SendEmail(request.Email, "Xác nhận Email reset mật khẩu","Nhấn vào nút này để vào reset mật khẩu tài khoản.",ConfirmLink,"Thay đổi");
                if (!EmailResponse) {
                    return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!", ValidateResult));
                }

                return Results.Ok(new Response(true, "", null));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra!", null));
            }

        }
    }
}

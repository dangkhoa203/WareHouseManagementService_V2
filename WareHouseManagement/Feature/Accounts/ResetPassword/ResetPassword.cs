using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Account;

namespace WareHouseManagement.Feature.Accounts.ResetPassword {
    public class ResetPassword : IEndpoint {
        public record Request(string NewPassword, string ConfirmNewPassword);
        public record Response(bool Success, string ErrorMessage, ValidationResult? Result);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.NewPassword).NotEmpty().WithMessage("Chưa nhập mật khẩu");
                RuleFor(r => r.NewPassword).MinimumLength(3).WithMessage("Phải nhập tối thiểu 3 ký tự");
                RuleFor(r => r.ConfirmNewPassword).Equal(r => r.NewPassword).WithMessage("Xác nhận mật khẩu mới chưa hợp lệ");
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPut("api/Account/PasswordReset/{userId}/{code}", Handler).WithTags("Account");
        }
        private static async Task<IResult> Handler([FromBody] Request request, [FromRoute] string userId, [FromRoute] string code, UserManager<Account> userManager) {
            try {
                var Validator = new Validator();
                var ValidateResult = await Validator.ValidateAsync(request);
                if (!ValidateResult.IsValid) {
                    return Results.BadRequest(new Response(false, "", ValidateResult));
                }

                Account User = await userManager.FindByIdAsync(Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(userId)));
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

                var result = await userManager.ResetPasswordAsync(User, code, request.NewPassword);
                if (!result.Succeeded) {
                    return Results.BadRequest(new Response(false, "Lỗi đã xảy ra", ValidateResult));
                }

                return Results.Ok(new Response(true, "", ValidateResult));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra", null));
            }
        }
    }
}

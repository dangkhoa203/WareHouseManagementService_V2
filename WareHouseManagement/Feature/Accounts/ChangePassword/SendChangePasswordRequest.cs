using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Middleware;
using WareHouseManagement.Model.Entity.Account;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Accounts.ChangePassword {
    public class SendChangePasswordRequest : IEndpoint {
        public record Request(string OldPassword, string NewPassword, string ConfirmNewPassword);
        public record Response(bool Success, string ErrorMessage, ValidationResult? Result);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.OldPassword).NotEmpty().WithMessage("Chưa nhập mật khẩu");
                RuleFor(r => r.OldPassword).MinimumLength(3).WithMessage("Phải nhập tối thiểu 3 ký tự");
                RuleFor(r => r.NewPassword).NotEmpty().WithMessage("Chưa nhập mật khẩu");
                RuleFor(r => r.NewPassword).MinimumLength(3).WithMessage("Phải nhập tối thiểu 3 ký tự");
                RuleFor(r => r.ConfirmNewPassword).Equal(r => r.NewPassword).WithMessage("Xác nhận mật khẩu mới chưa hợp lệ");
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Account/PasswordChange/", Handler).WithTags("Account");
        }
        [Authorize(Roles = Permission.Admin)]
        private static async Task<IResult> Handler(Request request, UserManager<Account> userManager, ClaimsPrincipal User) {
            try {
                var Validator = new Validator();
                var ValidateResult = await Validator.ValidateAsync(request);
                if (!ValidateResult.IsValid) {
                    return Results.BadRequest(new Response(false, "", ValidateResult));
                }
                Account UserDetail = await userManager.FindByNameAsync(User.Identity.Name);

                var Result = await userManager.CheckPasswordAsync(UserDetail, request.OldPassword);
                if (!Result) {
                    return Results.BadRequest(new Response(false, "Mật khẩu cũ không đúng với mật khẩu hiện tại!", ValidateResult));
                }

                var Token = await userManager.GeneratePasswordResetTokenAsync(UserDetail);
                Token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(Token));
                string Password = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(request.NewPassword));
                var ConfirmLink = $"https://localhost:7088/ConfirmDoiMatKhau/{WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(UserDetail.Id))}/{Password}/{Token}";
               
                bool EmailResponse = await EmailSender.SendEmail(UserDetail.Email, "Xác nhận Email thay đổi mật khẩu","Nhấn vào nút này để thay đổi mật khẩu tài khoản.",ConfirmLink,"Thay đổi");
                if (!EmailResponse) {
                    return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!", ValidateResult));
                }

                return Results.Ok(new Response(true, "", ValidateResult));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra!", null));
            }
        }
    }
}

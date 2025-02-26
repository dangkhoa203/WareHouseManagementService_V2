using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Account;

namespace WareHouseManagement.Feature.Accounts.ResetPassword {
    public class ResetPassword : IEndpoint
    {
        public record Request(string newPassword, string confirmNewPassword);
        public record Response(bool success, string errorMessage, ValidationResult? result);
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(r => r.newPassword).NotEmpty().WithMessage("Chưa nhập mật khẩu");
                RuleFor(r => r.newPassword).MinimumLength(3).WithMessage("Phải nhập tối thiểu 3 ký tự");
                RuleFor(r => r.confirmNewPassword).Equal(r => r.newPassword).WithMessage("Xác nhận mật khẩu mới chưa hợp lệ");
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/Account/PasswordReset/{userId}/{code}", Handler).WithTags("Account");
        }
        private static async Task<IResult> Handler(
            [FromBody] Request request,
            [FromRoute] string userId,
            [FromRoute] string code,
            UserManager<Account> userManager
            )
        {
            var validator = new Validator();
            var validateresult = await validator.ValidateAsync(request);
            if (!validateresult.IsValid)
            {
                return Results.BadRequest(new Response(false, "", validateresult));
            }
            Account user = await userManager.FindByIdAsync(Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(userId)));
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await userManager.ResetPasswordAsync(user, code, request.newPassword);
            if (!result.Succeeded)
            {
                return Results.BadRequest(new Response(false, "Lỗi đã xảy ra", validateresult));
            }
            return Results.Ok(new Response(true, "", validateresult));
        }
    }
}

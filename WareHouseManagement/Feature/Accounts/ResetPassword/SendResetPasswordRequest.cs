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
    public class SendResetPasswordRequest : IEndpoint
    {
        public record Request(string email);
        public record Response(bool success, string errorMessage, ValidationResult? validateError);
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(r => r.email).EmailAddress().WithMessage("Email chưa hợp lệ");
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/Account/PasswordReset/", Handler).WithTags("Account");
        }
        private static async Task<IResult> Handler(Request request, UserManager<Account> userManager)
        {
            var validator = new Validator();
            var validateresult = await validator.ValidateAsync(request);
            if (!validateresult.IsValid)
            {
                return Results.BadRequest(new Response(false, "Lỗi xảy ra", validateresult));
            }
            Account user = await userManager.FindByEmailAsync(request.email);
            if (user != null)
            {
                if (!await userManager.IsInRoleAsync(user, Permission.Admin)) {
                    return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!", null));
                }
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var ConfirmLink = $"https://localhost:7088/ResetPassword/{WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(user.Id))}/{token}";
                var body = $"Vào địa chỉ <a href='{ConfirmLink}'>này</a> để đặt lại mật khẩu.";
                bool emailResponse = await EmailSender.SendEmail(request.email, "Xác nhận Email reset mật khẩu", body);
                if (!emailResponse)
                {
                    return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!", null));
                }
                return Results.Ok(new Response(true, "", null));
            }
            return Results.NotFound(new Response(false, "Không có tài khoản với email này!", null));
        }
    }
}

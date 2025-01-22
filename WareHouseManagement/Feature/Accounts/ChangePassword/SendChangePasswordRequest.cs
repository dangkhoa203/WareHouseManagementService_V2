using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Middleware;
using WareHouseManagement.Model.Entity;

namespace WareHouseManagement.Feature.Accounts.ChangePassword
{
    public class SendChangePasswordRequest : IEndpoint
    {
        public record Request(string oldPassword,string newPassword,string confirmNewPassword);
        public record Response(bool success, string errorMessage, ValidationResult? result);
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(r => r.oldPassword).NotEmpty().WithMessage("Chưa nhập mật khẩu");
                RuleFor(r => r.oldPassword).MinimumLength(3).WithMessage("Phải nhập tối thiểu 3 ký tự");
                RuleFor(r => r.newPassword).NotEmpty().WithMessage("Chưa nhập mật khẩu");
                RuleFor(r => r.newPassword).MinimumLength(3).WithMessage("Phải nhập tối thiểu 3 ký tự");
                RuleFor(r => r.confirmNewPassword).Equal(r => r.newPassword).WithMessage("Xác nhận mật khẩu mới chưa hợp lệ");
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/Account/PasswordChange/", Handler).WithTags("Account");
        }
        private static async Task<IResult> Handler(Request request,UserManager<Account> userManager, ClaimsPrincipal user)
        {
            var validator = new Validator();
            var validateresult = await validator.ValidateAsync(request);
            if (!validateresult.IsValid)
            {
                return Results.BadRequest(new Response(false, "", validateresult));
            }
            Account userDetail = await userManager.FindByNameAsync(user.Identity.Name);

            var result = await userManager.CheckPasswordAsync(userDetail, request.oldPassword);
            if (!result)
            {
                return Results.BadRequest(new Response(false, "Mật khẩu cũ không đúng với mật khẩu hiện tại!", validateresult));
            }
            var token = await userManager.GeneratePasswordResetTokenAsync(userDetail);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            string password = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(request.newPassword));
            var confirmLink = $"https://localhost:7088/ConfirmDoiMatKhau/{WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(userDetail.Id))}/{password}/{token}";
            var body = $"Vào địa chỉ <a href='{confirmLink}'>này</a> để đổi mật khẩu.";
            bool emailResponse = await EmailSender.SendEmail(userDetail.Email, "Xác nhận Email thay đổi mật khẩu", body);
            if (!emailResponse)
            {
                return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!", null));
            }
            return Results.Ok(new Response(true, "", null));
        }
    }
}

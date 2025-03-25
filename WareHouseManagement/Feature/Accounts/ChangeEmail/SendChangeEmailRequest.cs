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

namespace WareHouseManagement.Feature.Accounts.Email {
    public class SendChangeEmailRequest : IEndpoint {
        public record Request(string OldEmail, string NewEmail);
        public record Response(bool Success, string ErrorMessage, ValidationResult? Result);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.NewEmail).EmailAddress().WithMessage("Email chưa hợp lệ");
            }
            public async Task<IResult> CheckValid(Request request, UserManager<Account> userManager, Account userDetail) {
                var ValidateResult = await ValidateAsync(request);
                if (!ValidateResult.IsValid)
                    return Results.BadRequest(new Response(false, "", ValidateResult));

                if (request.NewEmail == userDetail.Email)
                    return Results.BadRequest(new Response(false, "Email mới giống email cũ!", ValidateResult));

                if (request.OldEmail != userDetail.Email)
                    return Results.BadRequest(new Response(false, "Xác nhận email chưa phù hợp!", ValidateResult));

                if (await userManager.FindByEmailAsync(request.NewEmail) != null)
                    return Results.BadRequest(new Response(false, "email mới đang được sử dụng!", ValidateResult));

                return null;
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Account/EmailChange/", Handler).WithTags("Account");
        }
        [Authorize(Roles = Permission.Admin)]
        private static async Task<IResult> Handler(Request request, UserManager<Account> userManager, ClaimsPrincipal User) {
            try {
                var Validator = new Validator();
                Account userDetail = await userManager.FindByNameAsync(User.Identity.Name);
                var ValidateResult = await Validator.CheckValid(request, userManager, userDetail);
                if (ValidateResult != null)
                    return ValidateResult;

                var Token = await userManager.GenerateChangeEmailTokenAsync(userDetail, request.NewEmail);
                Token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(Token));

                string WebEmail = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(request.NewEmail));
                var ConfirmLink = $"https://localhost:7088/ConfirmDoiEmail/{WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(userDetail.Id))}/{WebEmail}/{Token}";

                bool EmailResponse = await EmailSender.SendEmail(userDetail.Email, "Xác nhận thay đổi email","Nhấn vào nút này để thay đổi email.",ConfirmLink,"Thay đổi");
                if (!EmailResponse) {
                    return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!", null));
                }

                return Results.Ok(new Response(true, "", null));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra!", null));
            }
        }
    }
}

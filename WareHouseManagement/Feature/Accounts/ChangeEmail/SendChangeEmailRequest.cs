using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Middleware;
using WareHouseManagement.Model.Entity;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Accounts.Email
{
    public class SendChangeEmailRequest : IEndpoint
    {
        public record Request(string oldEmail, string newEmail);
        public record Response(bool success, string errorMessage, ValidationResult? result);
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(r => r.newEmail).EmailAddress().WithMessage("Email chưa hợp lệ");
            }
            public async Task<IResult> checkValid(Request request, UserManager<Account> userManager, Account userDetail)
            {
                var validateresult = await ValidateAsync(request);
                if (!validateresult.IsValid)
                    return Results.BadRequest(new Response(false, "", validateresult));

                if (request.newEmail == userDetail.Email)
                    return Results.BadRequest(new Response(false, "Email mới giống email cũ!", validateresult));

                if (request.oldEmail != userDetail.Email)
                    return Results.BadRequest(new Response(false, "Xác nhận email chưa phù hợp!", validateresult));

                if (await userManager.FindByEmailAsync(request.newEmail) != null)
                    return Results.BadRequest(new Response(false, "email mới đang được sử dụng!", validateresult));

                return null;
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/Account/EmailChange/", Handler).WithTags("Account");
        }
        [Authorize(Roles =Permission.Admin)]
        private static async Task<IResult> Handler(Request request, UserManager<Account> userManager, ClaimsPrincipal user)
        {
            var validator = new Validator();
            Account userDetail = await userManager.FindByNameAsync(user.Identity.Name);
            var validateresult = await validator.checkValid(request,userManager,userDetail);
            if (validateresult!=null)
            {
                return validateresult;
            }

            var token = await userManager.GenerateChangeEmailTokenAsync(userDetail, request.newEmail);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            string WebEmail = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(request.newEmail));
            var confirmLink = $"https://localhost:7088/ConfirmDoiEmail/{WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(userDetail.Id))}/{WebEmail}/{token}";
            var body = $"Vào địa chỉ <a href='{confirmLink}'>này</a> để đổi email.";
            bool emailResponse = await EmailSender.SendEmail(userDetail.Email, "Xác nhận thay đổi email", body);
            if (!emailResponse)
            {
                return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!", null));
            }
            return Results.Ok(new Response(true, "", null));

        }
    }
}

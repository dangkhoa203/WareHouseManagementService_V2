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
using WareHouseManagement.Model.Entity;

namespace WareHouseManagement.Feature.Accounts
{
    public class Register : IEndpoint
    {
        public record Request(string userName, string password,string confirmPassword,string email,string fullName);
        public record Response(bool success, string errorMessage, ValidationResult? validateError);
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(r => r.userName).NotEmpty().WithMessage("Chưa nhập tên");
                RuleFor(r => r.password).NotEmpty().WithMessage("Chưa nhập mật khẩu");
                RuleFor(r => r.password).MinimumLength(3).WithMessage("Phải nhập tối thiểu 3 ký tự");
                RuleFor(r => r.email).EmailAddress().WithMessage("Email chưa hợp lệ");
                RuleFor(r => r.confirmPassword).Equal(r => r.password).WithMessage("Xác nhận mật khẩu chưa hợp lệ");
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/Account/Register", Handler).WithTags("Account");
        }
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, UserManager<Account> userManager, SignInManager<Account> signInManager, ClaimsPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                return Results.BadRequest(new Response(false,"Đã đăng nhập",null));
            }
            var validator = new Validator();
            var validateresult = await validator.ValidateAsync(request);
            if (!validateresult.IsValid)
            {
                return Results.BadRequest(new Response(false, "Lỗi xảy ra", validateresult));
            }
            if (await userManager.FindByEmailAsync(request.email) != null)
            {
                return Results.BadRequest(new Response(false, "Email đã có người đăng ký!", validateresult));
            }
            if (await userManager.FindByNameAsync(request.userName) != null)
            {
                return Results.BadRequest(new Response(false, "Tên đăng nhập đã có người đăng ký!", validateresult));
            }
           
            ServiceRegistered serviceRegistered = new ServiceRegistered();
            Account account = new()
            {
                Email = request.email,
                UserName = request.userName,
                FullName = request.fullName,
                ServiceRegistered = serviceRegistered,
            };
            var result = await userManager.CreateAsync(account, request.password);
            if (result.Succeeded)
            {
                var createduser = await userManager.FindByEmailAsync(request.email);
                //var token = await userManager.GenerateEmailConfirmationTokenAsync(createduser);
                //token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                //var ConfirmLink = $"https://localhost:7088/ConfirmEmail/{WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(createduser.UserName))}/{token}";
                //var body = $"Xác nhận email tại <a href='{ConfirmLink}'>đây</a>";
                //bool emailResponse = await EmailSender.SendEmail(createduser.Email,"Xác nhận Email tài khoản", body);
                //if (!emailResponse)
                //{
                //    context.Users.Remove(createduser);
                //    await context.SaveChangesAsync();
                //    return Results.BadRequest(new Response(false,"Lỗi đã xảy ra!",validateresult));
                //}
                createduser.EmailConfirmed=true;

                return Results.Ok(new Response(true,"", validateresult));
            }
            return Results.BadRequest(new Response(false,"Lỗi đã xảy ra", validateresult));
        }
    }
}

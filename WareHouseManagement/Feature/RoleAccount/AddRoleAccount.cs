using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Account;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.RoleAccount {
    public class AddRoleAccount : IEndpoint {
        public record Request(string UserName, string Password, string ConfirmPassword, string FullName, List<string> Roles);
        public record Response(bool Success, string ErrorMessage, ValidationResult? validateError);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.UserName).NotEmpty().WithMessage("Chưa nhập tên");
                RuleFor(r => r.Password).NotEmpty().WithMessage("Chưa nhập mật khẩu");
                RuleFor(r => r.Password).MinimumLength(3).WithMessage("Phải nhập tối thiểu 3 ký tự");
                RuleFor(r => r.ConfirmPassword).Equal(r => r.Password).WithMessage("Xác nhận mật khẩu chưa hợp lệ");
                RuleFor(r => r.Roles).NotEmpty().WithMessage("Chưa có quyền");
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Account/Role", Handler).WithTags("Account");
        }
        [Authorize(Roles = Permission.Admin)]
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, UserManager<Account> userManager, SignInManager<Account> signInManager, ClaimsPrincipal User) {
            try {
                var Validator = new Validator();
                var ValidateResult = await Validator.ValidateAsync(request);
                if (!ValidateResult.IsValid) {
                    return Results.BadRequest(new Response(false, "Lỗi xảy ra", ValidateResult));
                }
                if (await userManager.FindByNameAsync(request.UserName) != null) {
                    return Results.BadRequest(new Response(false, "Tên đăng nhập đang sử dụng!", ValidateResult));
                }

                var Service = await context.Users
                       .Include(u => u.ServiceRegistered)
                       .Where(u => u.UserName == User.Identity.Name)
                       .Select(u => u.ServiceRegistered)
                       .FirstOrDefaultAsync();

                Account Account = new() {
                    UserName = $"ACC{Nanoid.Generate(Nanoid.Alphabets.Digits, 5)}-{request.UserName}",
                    FullName = request.FullName,
                    EmailConfirmed = true,
                    ServiceRegistered = Service,
                };

                var Result = await userManager.CreateAsync(Account, request.Password);
                if (Result.Succeeded) {
                    var NewUser = await userManager.FindByNameAsync(Account.UserName);
                    await userManager.AddToRolesAsync(NewUser, request.Roles);
                    return Results.Ok(new Response(true, "", ValidateResult));
                }

                return Results.BadRequest(new Response(false, "Lỗi đã xảy ra", ValidateResult));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra", null));
            }
        }

    }
}

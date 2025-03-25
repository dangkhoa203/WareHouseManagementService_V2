using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Account;
using WareHouseManagement.Model.Entity.Customer_Entity;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.RoleAccount {
    public class UpdateRoleAccount : IEndpoint {
        public record Request(string Id, string UserName, string Password, string ConfirmPassword, string FullName, List<string> Roles);
        public record Response(bool Success, string ErrorMessage, ValidationResult? validateError);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.UserName).NotEmpty().WithMessage("Chưa nhập tên");
                RuleFor(r => r.Password).NotEmpty().WithMessage("Chưa nhập mật khẩu");
                RuleFor(r => r.Password).MinimumLength(3).WithMessage("Phải nhập tối thiểu 3 ký tự");
                RuleFor(r => r.ConfirmPassword).Equal(r => r.Password).WithMessage("Xác nhận mật khẩu chưa hợp lệ");
            }
            private record Checkmodel(string username,string fullname);
            public bool checkSame(Request request, Account Account) {
                Checkmodel NewDetail = new (request.UserName, request.FullName);
                Checkmodel OldDetail = new (Account.UserName,Account.FullName);
                return OldDetail == NewDetail;

            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPut("/api/Account/Role", Handler).WithTags("Account");
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

                var ServiceId = await context.Users
                       .Include(u => u.ServiceRegistered)
                       .Where(u => u.UserName == User.Identity.Name)
                       .Select(u => u.ServiceId)
                       .FirstOrDefaultAsync();

                Account Account = await userManager.FindByIdAsync(request.Id);
                if (Account != null) {
                    if (!Validator.checkSame(request, Account) && !await userManager.CheckPasswordAsync(Account, request.Password)) {
                        Account.UserName = $"{Account.UserName.Substring(0, 7)}-{request.UserName}";
                        var Token = await userManager.GeneratePasswordResetTokenAsync(Account);
                        await userManager.ChangePasswordAsync(Account, Token, request.Password);
                        Account.FullName = request.UserName;
                        if (await context.SaveChangesAsync() > 0) {
                            return Results.Ok(new Response(true, "", ValidateResult));
                        }
                    }
                    return Results.Ok(new Response(true, "", ValidateResult));
                }

                return Results.BadRequest(new Response(false, "Lỗi đã xảy ra", ValidateResult));
            }
            catch(Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra", null));
            }
        }
    }
}

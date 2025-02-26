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
        public record Request(string id, string userName, string password, string confirmPassword, string fullName, List<string> roles);
        public record Response(bool success, string errorMessage, ValidationResult? validateError);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.userName).NotEmpty().WithMessage("Chưa nhập tên");
                RuleFor(r => r.password).NotEmpty().WithMessage("Chưa nhập mật khẩu");
                RuleFor(r => r.password).MinimumLength(3).WithMessage("Phải nhập tối thiểu 3 ký tự");
                RuleFor(r => r.confirmPassword).Equal(r => r.password).WithMessage("Xác nhận mật khẩu chưa hợp lệ");
            }
            private record checkmodel(string username,string fullname);
            public bool checkSame(Request request, Account account) {
                checkmodel newDetail = new checkmodel(request.userName, request.fullName);
                checkmodel oldDetail = new checkmodel(account.UserName,account.FullName);
                return oldDetail == newDetail;

            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPut("/api/Account/Role", Handler).WithTags("Account");
        }
        [Authorize(Roles = Permission.Admin)]
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, UserManager<Account> userManager, SignInManager<Account> signInManager, ClaimsPrincipal user) {
            var validator = new Validator();
            var validateresult = await validator.ValidateAsync(request);
            if (!validateresult.IsValid) {
                return Results.BadRequest(new Response(false, "Lỗi xảy ra", validateresult));
            }
            if (await userManager.FindByNameAsync(request.userName) != null) {
                return Results.BadRequest(new Response(false, "Tên đăng nhập đang sử dụng!", validateresult));
            }

            var service = context.Users.Include(u => u.ServiceRegistered).Where(u => u.UserName == user.Identity.Name).Select(u => u.ServiceRegistered).FirstOrDefault();
            Account account = await userManager.FindByIdAsync(request.id);
            if (account != null) {
                if (!validator.checkSame(request, account) && !await userManager.CheckPasswordAsync(account, request.password)) {
                    account.UserName = $"{account.UserName.Substring(0,7)}-{request.userName}";
                    var token = await userManager.GeneratePasswordResetTokenAsync(account);
                    await userManager.ChangePasswordAsync(account, token, request.password);
                    account.FullName = request.userName;
                    if (await context.SaveChangesAsync() > 0) {
                        return Results.Ok(new Response(true, "", validateresult));
                    }
                }
                return Results.Ok(new Response(true, "", validateresult));
            }
            
            return Results.BadRequest(new Response(false, "Lỗi đã xảy ra", validateresult));
        }
    }
}

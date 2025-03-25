using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Account;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Accounts.ChangeFullName {
    public class ChangeFullname : IEndpoint {
        public record Request(string FullName);
        public record Response(bool Success, string ErrorMessage, ValidationResult? Result);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.FullName).NotEmpty().WithMessage("Chưa nhập tên");
                RuleFor(r => r.FullName).MinimumLength(3).WithMessage("Phải nhập tối thiểu 3 ký tự");
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPut("api/Account/FullNameChange/", Handler).WithTags("Account");
        }
        [Authorize(Roles = Permission.Admin)]
        private static async Task<IResult> Handler(Request request, UserManager<Account> userManager, ClaimsPrincipal User) {
            try {
                Account UserDetail = await userManager.FindByNameAsync(User.Identity.Name);

                var Validator = new Validator();
                var ValidateResult = await Validator.ValidateAsync(request);
                if (!ValidateResult.IsValid)
                    return Results.BadRequest(new Response(false, "", ValidateResult));

                UserDetail.FullName = request.FullName;
                var Result = await userManager.UpdateAsync(UserDetail);
                if (!Result.Succeeded) {
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

using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Product_Entity;
using WareHouseManagement.Model.Enum;
using WareHouseManagement.Model.Receipt;

namespace WareHouseManagement.Feature.Taxes {
    public class UpdateTax : IEndpoint {
        public record Request(string Id, string Name, string Description, float Percent);
        public record Response(bool Success, string ErrorMessage, ValidationResult? ValidateError);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.Name).NotEmpty().WithMessage("Chưa nhập tên");
                RuleFor(r => r.Percent).LessThanOrEqualTo(100).GreaterThanOrEqualTo(0).WithMessage("Phần trăm chưa hợp lệ!");
            }
            private record CheckModel(string Name, string Description, float Percent);
            public bool checkSame(Request request, Tax Tax) {
                CheckModel OldDetail = new (Tax.Name, Tax.Description, Tax.Percent);
                CheckModel NewDetail = new (request.Name, request.Description, request.Percent);
                return OldDetail == NewDetail;
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPut("/api/Taxes", Handler).WithTags("Taxes");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Tax)]
        private static async Task<IResult> Handler(Request request,ApplicationDbContext context,ClaimsPrincipal User) {
            try {
                var Validator = new Validator();
                var ValidatedResult = Validator.Validate(request);
                if (!ValidatedResult.IsValid)
                    return Results.BadRequest(new Response(false, "", ValidatedResult));

                var ServiceId = await context.Users
                        .Include(u => u.ServiceRegistered)
                        .Where(u => u.UserName == User.Identity.Name)
                        .Select(u => u.ServiceId)
                        .FirstOrDefaultAsync();

                var Tax = await context.Taxes
                    .Where(tax => tax.ServiceId == ServiceId)
                    .FirstOrDefaultAsync(tax => tax.Id == request.Id);
                if (Tax == null)
                    return Results.NotFound(new Response(false, "Lỗi xảy ra khi đang thực hiện!", ValidatedResult));

                if (!Validator.checkSame(request, Tax)) {
                    Tax.Name = request.Name;
                    Tax.Description = request.Description;
                    if (await context.SaveChangesAsync() < 1) {
                        return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", ValidatedResult));
                    }
                }
                return Results.Ok(new Response(true, "", ValidatedResult));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra!", null));
            }
        }
    }
}

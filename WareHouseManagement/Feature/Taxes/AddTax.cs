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
    public class AddTax:IEndpoint {
        public record Request(string Name, string Description,float Percent);
        public record Response(bool Success, string ErrorMessage, ValidationResult? ValidateError);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.Name).NotEmpty().WithMessage("Chưa nhập tên");
                RuleFor(r=>r.Percent).LessThanOrEqualTo(100).GreaterThanOrEqualTo(0).WithMessage("Phần trăm chưa hợp lệ!");
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Taxs", Handler).WithTags("Tax");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Tax)]
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var Validator = new Validator();
                var ValidatedResult = Validator.Validate(request);
                if (!ValidatedResult.IsValid) {
                    return Results.BadRequest(new Response(false, "", ValidatedResult));
                }

                var ServiceId = await context.Users
                       .Include(u => u.ServiceRegistered)
                       .Where(u => u.UserName == User.Identity.Name)
                       .Select(u => u.ServiceId)
                       .FirstOrDefaultAsync();

                Tax Tax = new() {
                    Name = request.Name,
                    Description = request.Description,
                    Percent = request.Percent,
                    ServiceId = ServiceId,
                };

                await context.Taxes.AddAsync(Tax);
                if (await context.SaveChangesAsync() > 0) {
                    return Results.Ok(new Response(true, "", ValidatedResult));
                }
                return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", ValidatedResult));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra!", null));
            }
        }
    }
}

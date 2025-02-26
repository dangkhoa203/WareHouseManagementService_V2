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
        public record Request(string name, string description,float percent);
        public record Response(bool success, string errorMessage, ValidationResult? error);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.name).NotEmpty().WithMessage("Chưa nhập tên");
                RuleFor(r=>r.percent).LessThanOrEqualTo(100).GreaterThanOrEqualTo(0).WithMessage("Phần trăm chưa hợp lệ!");
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Taxs", Handler).WithTags("Tax");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Tax)]
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, ClaimsPrincipal user) {
            var validator = new Validator();
            var validatedresult = validator.Validate(request);
            if (!validatedresult.IsValid) {
                return Results.BadRequest(new Response(false, "", validatedresult));
            }
            var serviceId = context.Users.Include(u => u.ServiceRegistered).Where(u => u.UserName == user.Identity.Name).Select(u => u.ServiceId).FirstOrDefault();
            Tax Tax = new() {
                Name = request.name,
                Description = request.description,
                Percent = request.percent,
                ServiceId = serviceId,
            };
            await context.Taxes.AddAsync(Tax);
            if (await context.SaveChangesAsync() > 0) {
                return Results.Ok(new Response(true, "", validatedresult));
            }
            return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", validatedresult));
        }
    }
}

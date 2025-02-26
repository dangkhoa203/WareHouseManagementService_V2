using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Model.Entity.Customer_Entity;
using WareHouseManagement.Model.Entity.Product_Entity;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Products {
    public class AddProduct {
        public record Request(string name, int pricePerUnit, string measureUnit, string? typeId);
        public record Response(bool success, string errorMessage, ValidationResult? error);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.name).NotEmpty().WithMessage("Chưa nhập tên");
                RuleFor(r => r.pricePerUnit).GreaterThan(-1).WithMessage("Giá chưa phù hợp");
                RuleFor(r => r.measureUnit).NotEmpty().WithMessage("Chưa nhập đơn vị tính");
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Products", Handler).WithTags("Products");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Product)]
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, ClaimsPrincipal user) {
            var serviceId = context.Users.Include(u => u.ServiceRegistered).Where(u => u.UserName == user.Identity.Name).Select(u => u.ServiceId).FirstOrDefault();
            var validator = new Validator();
            var validatedresult = validator.Validate(request);
            if (!validatedresult.IsValid) {
                return Results.BadRequest(new Response(false, "", validatedresult));
            }
            Product product = new() {
                Name = request.name,
                MeasureUnit = request.measureUnit,
                PricePerUnit = request.pricePerUnit,
                ProductType = await context.ProductTypes.FindAsync(request.typeId),
                ServiceId = serviceId,
            };
            await context.Products.AddAsync(product);
            if (await context.SaveChangesAsync() > 0) {
                return Results.Ok(new Response(true, "", validatedresult));
            }
            return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", validatedresult));
        }

    }
}

using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Model.Entity.Customer_Entity;
using WareHouseManagement.Model.Entity.Product_Entity;

namespace WareHouseManagement.Feature.Products {
    public class UpdateProduct {
        public record Request(string id, string name, int pricePerUnit, string measureUnit, string? typeId);
        public record Response(bool success, string errorMessage, ValidationResult? error);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.name).NotEmpty().WithMessage("Chưa nhập tên");
                RuleFor(r => r.pricePerUnit).GreaterThan(-1).WithMessage("Giá chưa phù hợp");
                RuleFor(r => r.measureUnit).NotEmpty().WithMessage("Chưa nhập đơn vị tính");
            }
            private record checkmodel(string name, int pricePerUnit, string measureUnit, string? typeId);
            public bool checkSame(Request request, Product product) {
                checkmodel newDetail = new checkmodel(request.name, request.pricePerUnit, request.measureUnit, request.typeId);
                checkmodel oldDetail = new checkmodel(product.Name, product.PricePerUnit, product.MeasureUnit, product.ProductType != null ? product.ProductType.Id : "");
                return oldDetail == newDetail;
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPut("/api/Products", Handler).RequireAuthorization().WithTags("Products");
        }
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, ClaimsPrincipal user) {

            var validator = new Validator();
            var validatedresult = validator.Validate(request);
            if (!validatedresult.IsValid) {
                return Results.BadRequest(new Response(false, "", validatedresult));
            }
            var service = context.Users.Include(u => u.ServiceRegistered).Where(u => u.UserName == user.Identity.Name).Select(u => u.ServiceRegistered).FirstOrDefault();
            var product = await context.Products
                .Include(p => p.ProductType)
                .Where(p => p.ServiceRegisteredFrom.Id == service.Id)
                .FirstOrDefaultAsync(p => p.Id == request.id);
            if (product == null)
                return Results.NotFound(new Response(false, "Lỗi xảy ra khi đang thực hiện!", validatedresult));

            if (!validator.checkSame(request, product)) {
                product.Name = request.name;
                product.MeasureUnit = request.measureUnit;
                product.PricePerUnit = request.pricePerUnit;
                product.ProductType = await context.ProductTypes.FindAsync(request.typeId);
                if (await context.SaveChangesAsync() < 1) {
                    return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", validatedresult));
                }
            }
            return Results.Ok(new Response(true, "", validatedresult));
        }
    }
}

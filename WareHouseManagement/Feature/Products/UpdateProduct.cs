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
    public class UpdateProduct {
        public record Request(string Id, string Name, int PricePerUnit, string MeasureUnit, string? TypeId);
        public record Response(bool Success, string ErrorMessage, ValidationResult? ValidateError);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.Name).NotEmpty().WithMessage("Chưa nhập tên");
                RuleFor(r => r.PricePerUnit).GreaterThan(-1).WithMessage("Giá chưa phù hợp");
                RuleFor(r => r.MeasureUnit).NotEmpty().WithMessage("Chưa nhập đơn vị tính");
            }
            private record Checkmodel(string name, float pricePerUnit, string measureUnit, string? typeId);
            public bool CheckSame(Request request, Product Product) {
                Checkmodel NewDetail = new (request.Name, request.PricePerUnit, request.MeasureUnit, request.TypeId);
                Checkmodel OldDetail = new (Product.Name, Product.PricePerUnit, Product.MeasureUnit, Product.ProductType != null ? Product.ProductType.Id : "");
                return OldDetail == NewDetail;
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPut("/api/Products", Handler).WithTags("Products");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Product)]
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

                var Product = await context.Products
                    .Include(product => product.ProductType)
                    .Where(product => product.ServiceId == ServiceId)
                    .FirstOrDefaultAsync(product => product.Id == request.Id);

                if (Product == null)
                    return Results.NotFound(new Response(false, "Lỗi xảy ra khi đang thực hiện!", ValidatedResult));

                if (!Validator.CheckSame(request, Product)) {
                    Product.Name = request.Name;
                    Product.MeasureUnit = request.MeasureUnit;
                    Product.PricePerUnit = request.PricePerUnit;
                    Product.ProductType = await context.ProductTypes.FindAsync(request.TypeId);
                    if (await context.SaveChangesAsync() < 1) {
                        return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", ValidatedResult));
                    }
                }

                return Results.Ok(new Response(true, "", ValidatedResult));
            }
            catch(Exception) {
                return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", null));
            }
          
        }
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Product_Entity;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Products {
    public class GetProduct : IEndpoint {
        public record TypeDTO(string Id, string Name, string Description);
        public record ProductDTO(string Id, string Name, float PricePerUnit, string MeasureUnit, TypeDTO? Type, DateTime DateCreated);
        public record Response(bool Success, ProductDTO Data, string ErrorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Products/{id}", Handler).WithTags("Products");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Product)]
        private static async Task<IResult> Handler(string id, ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                   .Include(u => u.ServiceRegistered)
                   .Where(u => u.UserName == User.Identity.Name)
                   .Select(u => u.ServiceId)
                   .FirstOrDefaultAsync();

                var Product = await context.Products
                    .Include(product => product.ProductType)
                    .Where(product => product.ServiceId == ServiceId)
                    .FirstOrDefaultAsync(product => product.Id == id);

                if (Product == null)
                    return Results.NotFound(new Response(false, null, "Không tìm thấy dữ liệu!"));
                if (Product.IsDeleted)
                    return Results.NotFound(new Response(false, null, "Dữ liệu đã xóa!"));

                var Data = new ProductDTO(
                        Product.Id,
                        Product.Name,
                        Product.PricePerUnit,
                        Product.MeasureUnit,
                        Product.ProductType != null ?
                        new TypeDTO(Product.ProductType.Id, Product.ProductType.Name, Product.ProductType.Description) : null,
                        Product.CreatedDate
                );
                return Results.Ok(new Response(true, Data, ""));
            }
            catch (Exception ex) {
                return Results.BadRequest(new Response(false, null, "Lỗi đã xảy ra!"));
            }
        }
    }
}

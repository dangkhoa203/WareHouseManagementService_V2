using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Products {
    public class GetProducts : IEndpoint {
        public record ProductDTO(string Id, string Name, float PricePerUnit, string MeasureUnit, string? TypeName, DateTime DateCreated);
        public record Response(bool Success, List<ProductDTO> data, string ErrorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Products/", Handler).WithTags("Products");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Product)]
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                   .Include(u => u.ServiceRegistered)
                   .Where(u => u.UserName == User.Identity.Name)
                   .Select(u => u.ServiceId)
                   .FirstOrDefaultAsync();

                var Products = await context.Products
                    .Include(product => product.ProductType)
                    .Where(product => product.ServiceId == ServiceId)
                    .Where(product=>!product.IsDeleted)
                    .OrderByDescending(product => product.CreatedDate)
                    .Select(product => new ProductDTO(
                        product.Id,
                        product.Name,
                        product.PricePerUnit,
                        product.MeasureUnit,
                        (product.ProductType != null ? product.ProductType.Name : ""),
                        product.CreatedDate
                        )
                    )
                    .ToListAsync();

                return Results.Ok(new Response(true, Products, ""));
            }
            catch (Exception ex) {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

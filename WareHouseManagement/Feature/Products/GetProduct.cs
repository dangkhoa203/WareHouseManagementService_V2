using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Products {
    public class GetProduct : IEndpoint {
        public record typeDTO(string id, string name, string description);
        public record productDTO(string id, string name, int pricePerUnit, string measureUnit, typeDTO? type, DateTime createDate);
        public record Response(bool success, productDTO data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Products/{id}", Handler).WithTags("Products");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Product)]
        private static async Task<IResult> Handler(string id, ApplicationDbContext context, ClaimsPrincipal user) {
            try {
                var service = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceRegistered)
                    .FirstOrDefault();
                var product = await context.Products
                    .Include(p => p.ProductType)
                    .Where(p => p.ServiceRegisteredFrom.Id == service.Id)
                    .Where(p => p.Id == id)
                    .OrderByDescending(p => p.CreatedDate)
                    .Select(p => new productDTO(
                        p.Id,
                        p.Name,
                        p.PricePerUnit,
                        p.MeasureUnit,
                        p.ProductType != null ?
                        new typeDTO(p.ProductType.Id, p.ProductType.Name, p.ProductType.Description) : null,
                        p.CreatedDate
                        )
                    )
                    .FirstOrDefaultAsync();
                return Results.Ok(new Response(true, product, ""));
            } catch (Exception ex) {
                return Results.BadRequest(new Response(false,null, "Lỗi đã xảy ra!"));
            }
        }
    }
}

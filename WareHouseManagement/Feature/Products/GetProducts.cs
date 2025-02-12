using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Products
{
    public class GetProducts:IEndpoint
    {
        public record productDTO(string id, string name, int pricePerUnit, string measureUnit, string? typeName, DateTime createDate);
        public record Response(bool success, List<productDTO> data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Products/", Handler).WithTags("Products");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Product)]
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal user) {
            try {
                var service = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceRegistered)
                    .FirstOrDefault();
                var products = await context.Products
                    .Include(p => p.ProductType)
                    .Where(p => p.ServiceRegisteredFrom.Id == service.Id)
                    .OrderByDescending(p => p.CreatedDate)
                    .Select(p => new productDTO(
                        p.Id,
                        p.Name,
                        p.PricePerUnit,
                        p.MeasureUnit,
                        (p.ProductType != null ? p.ProductType.Name : ""),
                        p.CreatedDate
                        )
                    )
                    .ToListAsync();
                return Results.Ok(new Response(true, products, ""));
            } catch (Exception ex) {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

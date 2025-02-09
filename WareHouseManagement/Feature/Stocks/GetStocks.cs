using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;

namespace WareHouseManagement.Feature.Stocks {
    public class GetStocks:IEndpoint {
        public record stockDTO(string productId, string productName,int quantity, string warehouseId, string warehouseName);
        public record Response(bool success, List<stockDTO> data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Stocks", Handler).RequireAuthorization().WithTags("Stocks");
        }
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal user) {
            try {
                var service = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceRegistered)
                    .FirstOrDefault();
                var stocks = await context.Stocks
                    .Where(s => s.ServiceRegisteredFrom.Id == service.Id)
                    .OrderByDescending(s => s.WarehouseId)
                    .Select(s => new stockDTO(s.ProductId, s.ProductNav.Name, s.Quantity,s.WarehouseId,s.WarehouseNav.Name))
                    .ToListAsync();
                return Results.Ok(new Response(true, stocks, ""));
            } catch (Exception ex) {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

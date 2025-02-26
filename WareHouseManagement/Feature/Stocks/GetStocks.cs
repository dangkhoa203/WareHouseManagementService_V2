using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Stocks {
    public class GetStocks:IEndpoint {
        public record stockDTO(string productId, string productName,int quantity, string warehouseId, string warehouseName);
        public record Response(bool success, List<stockDTO> data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Stocks", Handler).WithTags("Stocks");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Stock)]
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal user) {
            try {
                var serviceId = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceId)
                    .FirstOrDefault();
                var stocks = await context.Stocks
                    .Where(s => s.ServiceId == serviceId)
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

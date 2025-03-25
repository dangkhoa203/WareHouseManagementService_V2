using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Stocks {
    public class GetStocks : IEndpoint {
        public record StockDTO(string ProductId, string ProductName, int Quantity, string WarehouseId, string WarehouseName);
        public record Response(bool Success, List<StockDTO> data, string ErrorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Stocks", Handler).WithTags("Stocks");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Stock)]
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                   .Include(u => u.ServiceRegistered)
                   .Where(u => u.UserName == User.Identity.Name)
                   .Select(u => u.ServiceId)
                   .FirstOrDefaultAsync();

                var Stocks = await context.Stocks
                    .Where(stock => stock.ServiceId == ServiceId)
                    .OrderByDescending(stock => stock.WarehouseId)
                    .Include(stock => stock.ProductNav)
                    .Include(stock => stock.WarehouseNav)
                    .Select(stock => new StockDTO(stock.ProductId, stock.ProductNav.Name, stock.Quantity, stock.WarehouseId, stock.WarehouseNav.Name))
                    .ToListAsync();

                return Results.Ok(new Response(true, Stocks, ""));
            }
            catch (Exception ex) {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

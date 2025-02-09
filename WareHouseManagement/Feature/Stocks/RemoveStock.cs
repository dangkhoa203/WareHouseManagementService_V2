using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;

namespace WareHouseManagement.Feature.Stocks {
    public class RemoveStock:IEndpoint {
        public record Request(string productId,string warehouseId);
        public record Response(bool success, string errorMessage);
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapDelete("/api/Stocks/", Handler).RequireAuthorization().WithTags("Stocks");
        }
        private static async Task<IResult> Handler([FromBody] Request request, ApplicationDbContext context, ClaimsPrincipal user) {
            var service = context.Users
                .Include(u => u.ServiceRegistered)
                .Where(u => u.UserName == user.Identity.Name)
                .Select(u => u.ServiceRegistered)
                .FirstOrDefault();
            var stock = await context.Stocks
                .Where(s => s.ServiceRegisteredFrom.Id == service.Id)
                .FirstOrDefaultAsync(s => s.ProductId == request.productId && s.WarehouseId==request.warehouseId);
            if (stock != null) {
                context.Stocks.Remove(stock);
                var result = await context.SaveChangesAsync();
                if (result > 0)
                    return Results.Ok(new Response(true, ""));
                return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!"));
            }
            return Results.NotFound(new Response(false, "Không tìm thấy nhóm!"));
        }
    }
}

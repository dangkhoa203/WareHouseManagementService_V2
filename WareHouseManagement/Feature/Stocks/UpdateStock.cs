using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Customer_Entity;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Stocks {
    public class UpdateStock:IEndpoint {
        public record Request(string productID, string warehouseId, int quantity);
        public record Response(bool success, string errorMessage);
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPut("/api/Stocks", Handler).WithTags("Stocks");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Stock)]
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, ClaimsPrincipal user) {
            var service = context.Users
                .Include(u => u.ServiceRegistered)
                .Where(u => u.UserName == user.Identity.Name)
                .Select(u => u.ServiceRegistered)
                .FirstOrDefault();
            var stock = await context.Stocks
                          .Where(s => s.ServiceRegisteredFrom.Id == service.Id)
                          .FirstOrDefaultAsync(s => s.ProductId == request.productID && s.WarehouseId == request.warehouseId);
            if (stock == null)
                return Results.NotFound(new Response(false, "Lỗi xảy ra khi đang thực hiện!"));
            if(request.quantity != stock.Quantity) {
                stock.Quantity = request.quantity;
                if (await context.SaveChangesAsync() < 1) {
                    return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!"));
                }
            }
            return Results.Ok(new Response(true, ""));
        }
    }
}

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
        public record Request(string ProductID, string WarehouseId, int Quantity);
        public record Response(bool Success, string ErrorMessage);
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPut("/api/Stocks", Handler).WithTags("Stocks");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Stock)]
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                if (request.Quantity < 0)
                    return Results.BadRequest(new Response(false, "Số lượng không đủ!"));

                var ServiceId = await context.Users
                       .Include(u => u.ServiceRegistered)
                       .Where(u => u.UserName == User.Identity.Name)
                       .Select(u => u.ServiceId)
                       .FirstOrDefaultAsync();
                var Stock = await context.Stocks
                              .Where(stock => stock.ServiceId == ServiceId)
                              .FirstOrDefaultAsync(stock => stock.ProductId == request.ProductID && stock.WarehouseId == request.WarehouseId);
                if (Stock == null)
                    return Results.NotFound(new Response(false, "Lỗi xảy ra khi đang thực hiện!"));
                if (request.Quantity != Stock.Quantity) {
                    Stock.Quantity = request.Quantity;
                    if (await context.SaveChangesAsync() < 1) {
                        return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!"));
                    }
                }
                return Results.Ok(new Response(true, ""));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra!"));
            }
        }
    }
}

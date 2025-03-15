using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Customer_Entity;
using WareHouseManagement.Model.Entity.Warehouse_Entity;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Stocks {
    public class AddStocks : IEndpoint {
        public record Request(string ProductID, string WarehouseId, int Quantity);
        public record Response(bool Success, string ErrorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Stocks", Handler).WithTags("Stocks");
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

                var StockCheck = context.Stocks.FirstOrDefault(s => s.ProductId == request.ProductID && s.WarehouseId == request.WarehouseId);
                if (StockCheck != null) {
                    var Product = await context.Products
                                                .Where(product => product.ServiceId == ServiceId)
                                                .FirstOrDefaultAsync(product => product.Id == request.ProductID);

                    var warehouse = await context.Warehouses
                                                .Where(Warehouse => Warehouse.ServiceId == ServiceId)
                                                .FirstOrDefaultAsync(Warehouse => Warehouse.Id == request.ProductID);

                    if (Product == null || warehouse == null) {
                        return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!"));
                    }

                    Stock Stock = new Stock() {
                        ProductNav = Product,
                        WarehouseNav = warehouse,
                        Quantity = request.Quantity,
                        ServiceId = ServiceId
                    };
                    await context.Stocks.AddAsync(Stock);
                    if (await context.SaveChangesAsync() > 0) {
                        return Results.Ok(new Response(true, ""));
                    }
                }

                return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!"));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra!"));
            }
        }
    }
}

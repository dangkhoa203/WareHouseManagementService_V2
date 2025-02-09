using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity;
using WareHouseManagement.Model.Entity.Customer_Entity;

namespace WareHouseManagement.Feature.Stocks {
    public class AddStocks : IEndpoint {
        public record Request(string productID, string warehouseId, int quantity);
        public record Response(bool success, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Stocks", Handler).RequireAuthorization().WithTags("Stocks");
        }
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, ClaimsPrincipal user) {
            if(request.quantity<0)
                return Results.BadRequest(new Response(false, "Số lượng không đủ!"));
            var service = context.Users.Include(u => u.ServiceRegistered).Where(u => u.UserName == user.Identity.Name).Select(u => u.ServiceRegistered).FirstOrDefault();
            if(context.Stocks.FirstOrDefault(s=>s.ProductId==request.productID&& s.WarehouseId == request.warehouseId) != null) {
                var product = await context.Products.Where(p=>p.ServiceRegisteredFrom.Id == service.Id).FirstOrDefaultAsync(p=>p.Id==request.productID);
                var warehouse = await context.Warehouses.Where(w => w.ServiceRegisteredFrom.Id == service.Id).FirstOrDefaultAsync(w => w.Id == request.productID);
                if (product == null || warehouse == null) {
                    return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!"));
                }
                Stock stock = new Stock() {
                    ProductNav = product,
                    WarehouseNav = warehouse,
                    Quantity = request.quantity,
                };
                await context.Stocks.AddAsync(stock);
                if (await context.SaveChangesAsync() > 0) {
                    return Results.Ok(new Response(true, ""));
                }
            }
            return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!"));
        }
    }
}

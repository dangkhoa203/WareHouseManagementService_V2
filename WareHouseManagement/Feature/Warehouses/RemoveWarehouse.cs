using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Warehouses {
    public class RemoveWarehouse : IEndpoint {
        public record Request(string Id);
        public record Response(bool Success, string ErrorMessage);
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapDelete("/api/Warehouses/", Handler).WithTags("Warehouses");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Warehouse)]
        private static async Task<IResult> Handler([FromBody] Request request, ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                        .Include(u => u.ServiceRegistered)
                        .Where(u => u.UserName == User.Identity.Name)
                        .Select(u => u.ServiceId)
                        .FirstOrDefaultAsync();

                var Warehouse = await context.Warehouses
                    .Include(warehouse => warehouse.Stocks)
                    .Where(warehouse => warehouse.ServiceId == ServiceId)
                    .FirstOrDefaultAsync(warehouse => warehouse.Id == request.Id);

                if (Warehouse != null) {
                    if (Warehouse.Stocks.Count != 0) {
                        return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!"));
                    }
                    context.Warehouses.Remove(Warehouse);
                    var Result = await context.SaveChangesAsync();
                    if (Result > 0)
                        return Results.Ok(new Response(true, ""));
                    return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!"));
                }

                return Results.NotFound(new Response(false, "Không tìm thấy nhóm!"));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra!"));
            }
        }
    }
}

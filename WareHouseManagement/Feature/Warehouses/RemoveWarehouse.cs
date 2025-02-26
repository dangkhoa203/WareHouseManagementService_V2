using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Warehouses {
    public class RemoveWarehouse:IEndpoint {
        public record Request(string id);
        public record Response(bool success, string errorMessage);
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapDelete("/api/Wareehouses/", Handler).WithTags("Warehouses");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Warehouse)]
        private static async Task<IResult> Handler([FromBody] Request request, ApplicationDbContext context, ClaimsPrincipal user) {
            var serviceId = context.Users
                .Include(u => u.ServiceRegistered)
                .Where(u => u.UserName == user.Identity.Name)
                .Select(u => u.ServiceId)
                .FirstOrDefault();
            var warehouse = await context.Warehouses
                .Include(w=>w.Stocks)
                .Where(u => u.ServiceId == serviceId)
                .FirstOrDefaultAsync(u => u.Id == request.id);
            if (warehouse != null) {
                if (warehouse.Stocks.Count !=0) {
                    return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!"));
                }
                context.Warehouses.Remove(warehouse);
                var result = await context.SaveChangesAsync();
                if (result > 0)
                    return Results.Ok(new Response(true, ""));
                return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!"));
            }
            return Results.NotFound(new Response(false, "Không tìm thấy nhóm!"));
        }
    }
}

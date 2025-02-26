using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Warehouses {
    public class GetWarehouses : IEndpoint {
        public record warehouseDTO(string id, string name, string address, string city, DateTime createDate);
        public record Response(bool success, List<warehouseDTO> data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Warehouses/", Handler).WithTags("Warehouses");
        }   
        [Authorize(Roles = Permission.Admin + "," + Permission.Warehouse)]
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal user) {
            try {
                var serviceId = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceId)
                    .FirstOrDefault();
                var warehouse = await context.Warehouses
                    .Where(v => v.ServiceId == serviceId)
                    .OrderByDescending(v => v.CreatedDate)
                    .Select(v => new warehouseDTO(
                        v.Id,
                        v.Name,
                        v.Address,
                        v.City,
                        v.CreatedDate
                        )
                    )
                    .ToListAsync();
                return Results.Ok(new Response(true, warehouse, ""));
            } catch (Exception ex) {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

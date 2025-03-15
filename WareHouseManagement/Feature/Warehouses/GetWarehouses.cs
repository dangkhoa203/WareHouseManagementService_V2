using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Warehouses {
    public class GetWarehouses : IEndpoint {
        public record WarehouseDTO(string Id, string Name, string Address, string City, DateTime DateCreated);
        public record Response(bool Success, List<WarehouseDTO> Data, string ErrorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Warehouses/", Handler).WithTags("Warehouses");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Warehouse)]
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                   .Include(u => u.ServiceRegistered)
                   .Where(u => u.UserName == User.Identity.Name)
                   .Select(u => u.ServiceId)
                   .FirstOrDefaultAsync();

                var Warehouses = await context.Warehouses
                    .Where(warehouse => warehouse.ServiceId == ServiceId)
                    .Where(warehouse=>!warehouse.IsDeleted)
                    .OrderByDescending(warehouse => warehouse.CreatedDate)
                    .Select(warehouse => new WarehouseDTO(
                            warehouse.Id,
                            warehouse.Name,
                            warehouse.Address,
                            warehouse.City,
                            warehouse.CreatedDate
                        )
                    )
                    .ToListAsync();

                return Results.Ok(new Response(true, Warehouses, ""));
            }
            catch (Exception ex) {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

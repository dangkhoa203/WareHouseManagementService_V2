using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.VendorGroups {
    public class GetVendorGroups : IEndpoint {
        public record GroupDTO(string Id, string Name, DateTime DateCreated);
        public record Response(bool Success, List<GroupDTO> data, string ErrorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Vendor-Groups", Handler).WithTags("Vendor Groups");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Vendor)]
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                   .Include(u => u.ServiceRegistered)
                   .Where(u => u.UserName == User.Identity.Name)
                   .Select(u => u.ServiceId)
                   .FirstOrDefaultAsync();

                var Groups = await context.VendorGroups
                    .Where(group => group.ServiceId == ServiceId)
                    .Where(group=>!group.IsDeleted)
                    .OrderByDescending(group => group.CreatedDate)
                    .Select(group => new GroupDTO(group.Id, group.Name, group.CreatedDate))
                    .ToListAsync();

                return Results.Ok(new Response(true, Groups, ""));
            }
            catch (Exception ex) {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

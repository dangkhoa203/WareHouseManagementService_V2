using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.VendorGroups {
    public class GetVendorGroup : IEndpoint {
        public record GroupDTO(string Id, string Name, string Description, DateTime DateCreated);
        public record Response(bool Success, GroupDTO data, string ErrorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Vendor-Groups/{id}", Handler).WithTags("Vendor Groups");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Vendor)]
        private static async Task<IResult> Handler(string id, ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                   .Include(u => u.ServiceRegistered)
                   .Where(u => u.UserName == User.Identity.Name)
                   .Select(u => u.ServiceId)
                   .FirstOrDefaultAsync();

                var Group = await context.VendorGroups
                    .Where(group => group.ServiceId == ServiceId)
                    .FirstOrDefaultAsync(group => group.Id == id);

                if (Group == null)
                    return Results.NotFound(new Response(false, null, "Không tìm thấy dữ liệu!"));
                if(Group.IsDeleted)
                    return Results.NotFound(new Response(false, null, "Dữ liệu đã xóa!"));

                var Data = new GroupDTO(Group.Id, Group.Name, Group.Description, Group.CreatedDate);
                return Results.Ok(new Response(true, Data, ""));
            }
            catch (Exception ex) {
                return Results.BadRequest(new Response(false, null, "Lỗi đã xảy ra!"));
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.CustomerGroups {
    public class RemoveCustomerGroup : IEndpoint {
        public record Request(string Id);
        public record Response(bool Success, string ErrorMessage);
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapDelete("/api/Customer-Groups/", Handler).WithTags("Customer Groups");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Customer)]
        private static async Task<IResult> Handler([FromBody] Request request, ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                                             .Include(u => u.ServiceRegistered)
                                             .Where(u => u.UserName == User.Identity.Name)
                                             .Select(u => u.ServiceId)
                                             .FirstOrDefaultAsync();

                var Group = await context.CustomerGroups
                    .Where(group => group.ServiceId == ServiceId)
                    .FirstOrDefaultAsync(group => group.Id == request.Id);

                if (Group != null) {
                    Group.IsDeleted = true;
                    Group.DeletedAt = DateTime.Now;
                    var Result = await context.SaveChangesAsync();
                    if (Result > 0)
                        return Results.Ok(new Response(true, ""));
                    return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!"));
                }
                return Results.NotFound(new Response(false, "Không tìm thấy nhóm!"));
            }
            catch {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra!"));
            }
        }
    }
}

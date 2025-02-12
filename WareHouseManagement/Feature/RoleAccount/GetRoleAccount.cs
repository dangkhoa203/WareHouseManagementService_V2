using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.RoleAccount {
    public class GetRoleAccount:IEndpoint {
        public record accountDTO(string id, string userName, string fullName, DateTime createDate);
        public record Response(bool success, accountDTO data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Account/Role/{id}", Handler).WithTags("Account");
        }
        [Authorize(Roles = Permission.Admin)]
        private static async Task<IResult> Handler(string id,ApplicationDbContext context, ClaimsPrincipal user) {
            try {
                var service = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceRegistered)
                    .FirstOrDefault();
                var account = await context.Users
                    .Where(u => u.ServiceId == service.Id)
                    .Where(u=>u.Id==id)
                    .OrderByDescending(u => u.CreateDate)
                    .Select(u => new accountDTO(
                        u.Id,
                        u.UserName,
                        u.FullName,
                        u.CreateDate
                        )
                    )
                    .FirstOrDefaultAsync();
                if (account != null) {
                    return Results.Ok(new Response(true, account, ""));
                }
                return Results.NotFound(new Response(true, null, "Không tìm thấy!"));
            } catch (Exception ex) {
                return Results.BadRequest(new Response(false, null, "Lỗi đã xảy ra!"));
            }
        }
    }
}

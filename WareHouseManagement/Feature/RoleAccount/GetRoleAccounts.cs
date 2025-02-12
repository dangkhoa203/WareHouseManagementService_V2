using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.RoleAccount {
    public class GetRoleAccounts : IEndpoint {
        public record accountDTO(string id, string userName, string fullName, DateTime createDate);
        public record Response(bool success, List<accountDTO> data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Account/Role", Handler).WithTags("Account");
        }
        [Authorize(Roles = Permission.Admin)]
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal user) {
            try {
                var service = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceRegistered)
                    .FirstOrDefault();
                var accounts = await context.Users
                    .Where(u => u.ServiceId == service.Id)
                    .OrderByDescending(u => u.CreateDate)
                    .Select(u => new accountDTO(
                        u.Id,
                        u.UserName,
                        u.FullName,
                        u.CreateDate
                        )
                    )
                    .ToListAsync();
                return Results.Ok(new Response(true, accounts, ""));
            } catch (Exception ex) {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

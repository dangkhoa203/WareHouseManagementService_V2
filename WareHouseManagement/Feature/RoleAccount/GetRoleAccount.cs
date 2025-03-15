using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.RoleAccount {
    public class GetRoleAccount : IEndpoint {
        public record AccountDTO(string Id, string UserName, string FullName, DateTime DateCreated);
        public record Response(bool Success, AccountDTO Data, string ErrorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Account/Role/{id}", Handler).WithTags("Account");
        }
        [Authorize(Roles = Permission.Admin)]
        private static async Task<IResult> Handler(string id, ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == User.Identity.Name)
                    .Select(u => u.ServiceId)
                    .FirstOrDefaultAsync();

                var Account = await context.Users
                    .Where(account => account.ServiceId == ServiceId)
                    .Where(account => account.Id == id)
                    .OrderByDescending(account => account.DateCreated)
                    .Select(account => new AccountDTO(
                        account.Id,
                        account.UserName,
                        account.FullName,
                        account.DateCreated
                        )
                    )
                    .FirstOrDefaultAsync();

                if (Account != null) {
                    return Results.Ok(new Response(true, Account, ""));
                }

                return Results.NotFound(new Response(true, null, "Không tìm thấy!"));
            }
            catch (Exception ex) {
                return Results.BadRequest(new Response(false, null, "Lỗi đã xảy ra!"));
            }
        }
    }
}

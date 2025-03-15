using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.RoleAccount {
    public class GetRoleAccounts : IEndpoint {
        public record AccountDTO(string Id, string UserName, string FullName, DateTime DateCreated);
        public record Response(bool Success, List<AccountDTO> Data, string ErrorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Account/Role", Handler).WithTags("Account");
        }
        [Authorize(Roles = Permission.Admin)]
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                   .Include(u => u.ServiceRegistered)
                   .Where(u => u.UserName == User.Identity.Name)
                   .Select(u => u.ServiceId)
                   .FirstOrDefaultAsync();

                var Accounts = await context.Users
                    .Where(account => account.ServiceId == ServiceId)
                    .OrderByDescending(account => account.DateCreated)
                    .Select(account => new AccountDTO(
                        account.Id,
                        account.UserName,
                        account.FullName,
                        account.DateCreated
                        )
                    )
                    .ToListAsync();

                return Results.Ok(new Response(true, Accounts, ""));
            }
            catch (Exception ex) {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

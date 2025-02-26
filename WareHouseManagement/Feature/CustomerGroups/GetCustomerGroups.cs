using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.CustomerGroups {
    public class GetCustomerGroups : IEndpoint {
        public record groupDTO(string id, string name, string description, DateTime createDate);
        public record Response(bool success, List<groupDTO> data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Customer-Groups", Handler).WithTags("Customer Groups");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Customer)]
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal user) {
            try {
                var serviceId = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceId)
                    .FirstOrDefault();
                var groups = await context.CustomerGroups
                    .Where(u => u.ServiceId == serviceId)
                    .OrderByDescending(u => u.CreatedDate)
                    .Select(u => new groupDTO(u.Id, u.Name, u.Description, u.CreatedDate))
                    .ToListAsync();
                return Results.Ok(new Response(true, groups, ""));
            } catch (Exception ex) {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

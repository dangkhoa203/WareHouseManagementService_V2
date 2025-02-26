using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Customers {
    public class GetCustomers : IEndpoint {
        public record customerDTO(string id, string name, string email, string address, string phoneNumber, string groupName, DateTime createDate);
        public record Response(bool success, List<customerDTO> data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Customers/", Handler).RequireAuthorization().WithTags("Customers");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Customer)]
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal user) {
            try {
                var serviceId = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceId)
                    .FirstOrDefault();
                var customers = await context.Customers
                    .Include(c => c.CustomerGroup)
                    .Where(c => c.ServiceId == serviceId)
                    .OrderByDescending(c => c.CreatedDate)
                    .Select(c => new customerDTO(
                        c.Id,
                        c.Name,
                        c.Email,
                        c.Address,
                        c.PhoneNumber,
                        (c.CustomerGroup != null ? c.CustomerGroup.Name : ""),
                        c.CreatedDate
                        )
                    )
                    .ToListAsync();
                return Results.Ok(new Response(true, customers, ""));
            } catch (Exception ex) {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

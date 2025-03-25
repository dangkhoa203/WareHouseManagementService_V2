using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Customers {
    public class GetCustomers : IEndpoint {
        public record CustomerDTO(string Id, string Name, string Email, string Address, string PhoneNumber, string GroupName, DateTime DateCreated);
        public record Response(bool Success, List<CustomerDTO> Data, string ErrorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Customers/", Handler).RequireAuthorization().WithTags("Customers");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Customer)]
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                   .Include(u => u.ServiceRegistered)
                   .Where(u => u.UserName == User.Identity.Name)
                   .Select(u => u.ServiceId)
                   .FirstOrDefaultAsync();

                var Customers = await context.Customers
                    .Include(customer => customer.CustomerGroup)
                    .Where(customer => customer.ServiceId == ServiceId)
                    .Where(customer=>!customer.IsDeleted)
                    .OrderByDescending(customer => customer.CreatedDate)
                    .Select(customer => new CustomerDTO(
                        customer.Id,
                        customer.Name,
                        customer.Email,
                        customer.Address,
                        customer.PhoneNumber,
                        (customer.CustomerGroup != null ? customer.CustomerGroup.Name : ""),
                        customer.CreatedDate
                        )
                    )
                    .ToListAsync();

                return Results.Ok(new Response(true, Customers, ""));
            }
            catch (Exception ex) {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

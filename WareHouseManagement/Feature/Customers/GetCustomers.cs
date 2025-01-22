using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;

namespace WareHouseManagement.Feature.Customers
{
    public class GetCustomers:IEndpoint
    {
        public record customerDTO(string id,string name,string email,string address,string phoneNumber,string groupName,DateTime createDate);
        public record Response(bool success, List<customerDTO> data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/Customers/",Handler).WithTags("Customers");
        }
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal user)
        {
            try
            {
                var service = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceRegistered)
                    .FirstOrDefault();
                var customers = await context.Customers
                    .Include(c=>c.CustomerGroup)
                    .Where(c => c.ServiceRegisteredFrom.Id == service.Id)
                    .OrderByDescending(c => c.CreatedDate)
                    .Select(c => new customerDTO(
                        c.Id, 
                        c.Name,
                        c.Email,
                        c.Address,
                        c.PhoneNumber,
                        (c.CustomerGroup!=null ? c.CustomerGroup.Name:""),
                        c.CreatedDate
                        )
                    )
                    .ToListAsync();
                return Results.Ok(new Response(true, customers, ""));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

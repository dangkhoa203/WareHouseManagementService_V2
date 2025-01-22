using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;

namespace WareHouseManagement.Feature.CustomerGroups
{
    public class GetCustomerGroups:IEndpoint
    {
        public record groupDTO(string id, string name, string description, DateTime createDate);
        public record Response(bool success,List<groupDTO> data, string errorMessage);
       
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/Customer-Groups", Handler).WithTags("CustomerGroups");
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
                var groups = await context.CustomerGroups
                    .Where(g => g.ServiceRegisteredFrom.Id == service.Id)
                    .OrderByDescending(g => g.CreatedDate)
                    .Select(g=>new groupDTO(g.Id,g.Name,g.Description,g.CreatedDate))
                    .ToListAsync();
                return Results.Ok(new Response(true, groups, ""));
            }
            catch (Exception ex) {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

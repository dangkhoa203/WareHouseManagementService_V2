using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;

namespace WareHouseManagement.Feature.VendorGroups
{
    public class GetVendorGroups : IEndpoint
    {
        public record groupDTO(string id, string name, string description, DateTime createDate);
        public record Response(bool success, List<groupDTO> data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/Vendor-Groups", Handler).RequireAuthorization().WithTags("Vendor Groups");
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
                var groups = await context.VendorGroups
                    .Where(t => t.ServiceRegisteredFrom.Id == service.Id)
                    .OrderByDescending(t => t.CreatedDate)
                    .Select(t => new groupDTO(t.Id, t.Name, t.Description, t.CreatedDate))
                    .ToListAsync();
                return Results.Ok(new Response(true, groups, ""));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

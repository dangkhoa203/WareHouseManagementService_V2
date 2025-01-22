using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;

namespace WareHouseManagement.Feature.ProductTypes
{
    public class GetProductTypes:IEndpoint
    {
        public record typeDTO(string id, string name, string description, DateTime createDate);
        public record Response(bool success, List<typeDTO> data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/Product-Types", Handler).WithTags("ProductTypes");
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
                var types = await context.ProductTypes
                    .Where(g => g.ServiceRegisteredFrom.Id == service.Id)
                    .OrderByDescending(g => g.CreatedDate)
                    .Select(g => new typeDTO(g.Id, g.Name, g.Description, g.CreatedDate))
                    .ToListAsync();
                return Results.Ok(new Response(true, types, ""));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

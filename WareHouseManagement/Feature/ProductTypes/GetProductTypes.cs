using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.ProductTypes
{
    public class GetProductTypes:IEndpoint
    {
        public record typeDTO(string id, string name, string description, DateTime createDate);
        public record Response(bool success, List<typeDTO> data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/Product-Types", Handler).WithTags("Product Types");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Product)]
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal user)
        {
            try
            {
                var serviceId = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceId)
                    .FirstOrDefault();
                var types = await context.ProductTypes
                    .Where(u => u.ServiceId == serviceId)
                    .OrderByDescending(u => u.CreatedDate)
                    .Select(u => new typeDTO(u.Id, u.Name, u.Description, u.CreatedDate))
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

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.ProductTypes
{
    public class GetProductType : IEndpoint
    {
        public record typeDTO(string id, string name, string description, DateTime createDate);
        public record Response(bool success, typeDTO data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/Product-Types/{id}", Handler).WithTags("Product Types");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Product)]
        private static async Task<IResult> Handler(string id, ApplicationDbContext context, ClaimsPrincipal user)
        {
            try
            {
                var service = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceRegistered)
                    .FirstOrDefault();
                var types = await context.ProductTypes
                    .Where(u => u.ServiceRegisteredFrom.Id == service.Id)
                    .Where(u => u.Id == id)
                    .Select(u => new typeDTO(u.Id, u.Name, u.Description, u.CreatedDate))
                    .FirstOrDefaultAsync();
                if (types != null)
                    return Results.Ok(new Response(true, types, ""));
                return Results.NotFound(new Response(false, null, "Không tìm thấy dữ liệu!"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new Response(false, null, "Lỗi đã xảy ra!"));
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.ProductTypes {
    public class GetProductTypes : IEndpoint {
        public record TypeDTO(string Id, string Name, DateTime DateCreated);
        public record Response(bool Success, List<TypeDTO> Data, string ErrorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Product-Types", Handler).WithTags("Product Types");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Product)]
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                   .Include(u => u.ServiceRegistered)
                   .Where(u => u.UserName == User.Identity.Name)
                   .Select(u => u.ServiceId)
                   .FirstOrDefaultAsync();

                var Types = await context.ProductTypes
                    .Where(type => type.ServiceId == ServiceId)
                    .Where(type=>!type.IsDeleted)
                    .OrderByDescending(type => type.CreatedDate)
                    .Select(type => new TypeDTO(type.Id, type.Name, type.CreatedDate))
                    .ToListAsync();

                return Results.Ok(new Response(true, Types, ""));
            }
            catch (Exception ex) {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

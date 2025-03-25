using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.ProductTypes {
    public class RemoveProductType : IEndpoint {
        public record Request(string Id);
        public record Response(bool Success, string ErrorMessage);
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapDelete("/api/Product-Types/", Handler).WithTags("Product Types");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Product)]
        private static async Task<IResult> Handler([FromBody] Request request, ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                       .Include(u => u.ServiceRegistered)
                       .Where(u => u.UserName == User.Identity.Name)
                       .Select(u => u.ServiceId)
                       .FirstOrDefaultAsync();

                var Type = await context.ProductTypes
                    .Where(type => type.ServiceId == ServiceId)
                    .FirstOrDefaultAsync(type => type.Id == request.Id);

                if (Type != null) {
                    Type.IsDeleted = true;
                    Type.DeletedAt = DateTime.Now;
                    var Result = await context.SaveChangesAsync();
                    if (Result > 0)
                        return Results.Ok(new Response(true, ""));
                    return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!"));
                }

                return Results.NotFound(new Response(false, "Không tìm thấy nhóm!"));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra!"));
            }
        }
    }
}

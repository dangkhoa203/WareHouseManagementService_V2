using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;

namespace WareHouseManagement.Feature.VendorReplenishReceipts {
    public class RemoveVendorReceipt:IEndpoint {
        public record Request(string id);
        public record Response(bool success, string errorMessage);
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapDelete("/api/Vendor-Receipts/", Handler).RequireAuthorization().RequireAuthorization().WithTags("Vendor Receipts");
        }
        private static async Task<IResult> Handler([FromBody] Request request, ApplicationDbContext context, ClaimsPrincipal user) {
            var service = context.Users
                .Include(u => u.ServiceRegistered)
                .Where(u => u.UserName == user.Identity.Name)
                .Select(u => u.ServiceRegistered)
                .FirstOrDefault();
            var receipt = await context.VendorReplenishReceipts
                .Where(t => t.ServiceRegisteredFrom.Id == service.Id)
                .FirstOrDefaultAsync(t => t.Id == request.id);
            if (receipt != null) {
                receipt.IsDeleted = true;
                var result = await context.SaveChangesAsync();
                if (result > 0)
                    return Results.Ok(new Response(true, ""));
                return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!"));
            }
            return Results.NotFound(new Response(false, "Không tìm thấy nhóm!"));
        }
    }
}

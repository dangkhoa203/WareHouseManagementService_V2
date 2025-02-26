using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.VendorReplenishReceipts {
    public class RemoveVendorReceipt:IEndpoint {
        public record Request(string id);
        public record Response(bool success, string errorMessage);
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapDelete("/api/Vendor-Receipts/", Handler).WithTags("Vendor Receipts");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.VendorReceipt)]
        private static async Task<IResult> Handler([FromBody] Request request, ApplicationDbContext context, ClaimsPrincipal user) {
            var serviceId = context.Users
                .Include(u => u.ServiceRegistered)
                .Where(u => u.UserName == user.Identity.Name)
                .Select(u => u.ServiceId)
                .FirstOrDefault();
            var receipt = await context.VendorReplenishReceipts
                .Where(u => u.ServiceId == serviceId)
                .FirstOrDefaultAsync(u => u.Id == request.id);
            if (receipt != null) {
                receipt.IsDeleted = true;
                receipt.DeletedAt = DateTime.Now;
                var result = await context.SaveChangesAsync();
                if (result > 0)
                    return Results.Ok(new Response(true, ""));
                return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!"));
            }
            return Results.NotFound(new Response(false, "Không tìm thấy nhóm!"));
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.VendorReplenishReceipts {
    public class UpdateVendorReceiptStatus : IEndpoint {
        public record Request(string id, StatusEnum status);
        public record Response(bool success, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPut("/api/Vendor-Receipts", Handler).WithTags("Vendor Receipts");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.CustomerReceipt)]
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, ClaimsPrincipal user) {


            var serviceId = context.Users
                .Include(u => u.ServiceRegistered)
                .Where(u => u.UserName == user.Identity.Name)
                .Select(u => u.ServiceId)
                .FirstOrDefault();

            var receipt = await context.VendorReplenishReceipts
                .Where(u => u.ServiceId == serviceId)
                .Where(r => !r.IsDeleted)
                .FirstOrDefaultAsync(u => u.Id == request.id);
            if (receipt == null)
                return Results.NotFound(new Response(false, "Lỗi xảy ra khi đang thực hiện!"));
            if (request.status != receipt.Status) {
                receipt.Status = request.status;
                if (await context.SaveChangesAsync() < 1) {
                    return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!"));
                }
            }
            return Results.Ok(new Response(true, ""));
        }
    }
}

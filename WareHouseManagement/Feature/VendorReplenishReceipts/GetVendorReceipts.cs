using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.VendorReplenishReceipts {
    public class GetVendorReceipts:IEndpoint {
        public record vendorDTO(string id, string name, string email, string address, string phoneNumber);
        public record receiptDTO(string id, vendorDTO customer, DateTime dateOfOrder);
        public record Response(bool success, List<receiptDTO> data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Vendor-Receipts", Handler).WithTags("Vendor Receipts");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.VendorReceipt)]
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal user) {
            try {
                var serviceId = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceId)
                    .FirstOrDefault();
                var receipts = await context.VendorReplenishReceipts
                    .Include(re => re.Vendor)
                    .Where(re => re.ServiceId == serviceId)
                    .OrderByDescending(re => re.CreatedDate)
                    .Select(re => new receiptDTO(
                        re.Id,
                        new vendorDTO(
                            re.Vendor.Id,
                            re.Vendor.Name,
                            re.Vendor.Email,
                            re.Vendor.Address,
                            re.Vendor.PhoneNumber
                            ),
                        re.DateOrder
                    ))
                    .ToListAsync();
                return Results.Ok(new Response(true, receipts, ""));
            } catch (Exception ex) {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

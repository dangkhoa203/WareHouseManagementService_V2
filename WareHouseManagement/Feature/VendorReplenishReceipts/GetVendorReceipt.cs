using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.VendorReplenishReceipts {
    public class GetVendorReceipt:IEndpoint {
        public record vendorDTO(string id, string name, string email, string address, string phoneNumber, string groupName);
        public record taxDTO(string id, string description, float percent);
        public record detailDTO(string productID, string productName, float price, int quantity, float totalPrice);
        public record receiptDTO(vendorDTO vendor, ICollection<detailDTO> details,taxDTO? tax, DateTime dateOfOrder, DateTime createDate);
        public record Response(bool success, receiptDTO? data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Vendor-Receipts/{id}", Handler).WithTags("Vendor Receipts");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.VendorReceipt)]
        private static async Task<IResult> Handler(string id, ApplicationDbContext context, ClaimsPrincipal user) {
            try {
                var serviceId = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceId)
                    .FirstOrDefault();
                var Receipt = await context.VendorReplenishReceipts
                    .Include(r => r.Tax)
                    .Include(r => r.Details)
                    .ThenInclude(d => d.ProductNav)
                    .Include(r => r.Vendor)
                    .ThenInclude(c => c.VendorGroup)
                    .Where(u => u.ServiceId == serviceId)
                    .Where(r => !r.IsDeleted)
                    .FirstOrDefaultAsync(r => r.Id == id);
                if (Receipt != null) {
                    var vendor = new vendorDTO(
                        Receipt.Vendor.Id,
                        Receipt.Vendor.Name,
                        Receipt.Vendor.Email,
                        Receipt.Vendor.Address,
                        Receipt.Vendor.PhoneNumber,
                        Receipt.Vendor.VendorGroup !=null ? Receipt.Vendor.VendorGroup.Name :null
                    );
                    var details = Receipt.Details
                    .Select(
                        d => new detailDTO(
                        d.ProductId,
                        d.ProductNav.Name,
                        d.PriceOfOne,
                        d.Quantity,
                        d.TotalPrice
                        )
                    )
                    .ToList();
                    var tax = Receipt.Tax != null ? new taxDTO(Receipt.Tax.Id, Receipt.Tax.Name, Receipt.Tax.Percent) : null;
                    var receipt = new receiptDTO(vendor, details,tax, Receipt.DateOrder, Receipt.CreatedDate);
                    return Results.Ok(new Response(true, receipt, ""));
                }
                return Results.NotFound(new Response(false, null, "Không tìm thấy dữ liệu!"));
            } catch (Exception ex) {
                return Results.BadRequest(new Response(false, null, "Lỗi đã xảy ra!"));
            }
        }
    }
}

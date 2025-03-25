using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Vendor_EntiTy;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Vendors {
    public class GetVendor : IEndpoint {
        public record GroupDTO(string Id, string Name, string Description);
        public record VendorDTO(string Id, string Name, string Email, string Address, string PhoneNumber, GroupDTO? Group, DateTime DateCreated);
        public record Response(bool Success, VendorDTO data, string ErrorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Vendors/{id}", Handler).WithTags("Vendors");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Vendor)]
        private static async Task<IResult> Handler(string id, ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                   .Include(u => u.ServiceRegistered)
                   .Where(u => u.UserName == User.Identity.Name)
                   .Select(u => u.ServiceId)
                   .FirstOrDefaultAsync();

                var Vendor = await context.Vendors
                    .Include(vendor => vendor.VendorGroup)
                    .Where(vendor => vendor.ServiceId == ServiceId)
                    .FirstOrDefaultAsync(vendor => vendor.Id == id);

                if (Vendor == null)
                    return Results.NotFound(new Response(false, null, "Không tìm thấy dữ liệu!"));
                if (Vendor.IsDeleted)
                    return Results.NotFound(new Response(false, null, "Dữ liệu đã xóa!"));

                var Data = new VendorDTO(
                        Vendor.Id,
                        Vendor.Name,
                        Vendor.Email,
                        Vendor.Address,
                        Vendor.PhoneNumber,
                        Vendor.VendorGroup != null ? new GroupDTO(Vendor.VendorGroup.Id, Vendor.VendorGroup.Name, Vendor.VendorGroup.Description) : null,
                        Vendor.CreatedDate
                );
                return Results.Ok(new Response(true, Data, ""));
            }
            catch (Exception ex) {
                return Results.BadRequest(new Response(false, null, "Lỗi đã xảy ra!"));
            }
        }
    }
}

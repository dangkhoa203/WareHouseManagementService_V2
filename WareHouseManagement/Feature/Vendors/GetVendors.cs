using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Vendors {
    public class GetVendors : IEndpoint {
        public record VendorDTO(string Id, string Name, string Email, string Address, string PhoneNumber, string GroupName, DateTime DateCreated);
        public record Response(bool Success, List<VendorDTO> Data, string ErrorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Vendors/", Handler).WithTags("Vendors");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Vendor)]
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                   .Include(u => u.ServiceRegistered)
                   .Where(u => u.UserName == User.Identity.Name)
                   .Select(u => u.ServiceId)
                   .FirstOrDefaultAsync();

                var Vendors = await context.Vendors
                    .Include(vendor => vendor.VendorGroup)
                    .Where(vendor => vendor.ServiceId == ServiceId)
                    .Where(vendor=>!vendor.IsDeleted)
                    .OrderByDescending(vendor => vendor.CreatedDate)
                    .Select(vendor => new VendorDTO(
                        vendor.Id,
                        vendor.Name,
                        vendor.Email,
                        vendor.Address,
                        vendor.PhoneNumber,
                        vendor.VendorGroup != null ? vendor.VendorGroup.Name : "",
                        vendor.CreatedDate
                        )
                    )
                    .ToListAsync();

                return Results.Ok(new Response(true, Vendors, ""));
            }
            catch (Exception ex) {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

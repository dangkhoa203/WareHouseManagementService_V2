using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;

namespace WareHouseManagement.Feature.Vendors
{
    public class GetVendors:IEndpoint
    {
        public record vendorDTO(string id, string name, string email, string address, string phoneNumber, string groupName, DateTime createDate);
        public record Response(bool success, List<vendorDTO> data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/Vendors/", Handler).WithTags("Vendors");
        }
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal user)
        {
            try
            {
                var service = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceRegistered)
                    .FirstOrDefault();
                var vendors = await context.Vendors
                    .Include(v => v.VendorGroup)
                    .Where(v => v.ServiceRegisteredFrom.Id == service.Id)
                    .OrderByDescending(v => v.CreatedDate)
                    .Select(v => new vendorDTO(
                        v.Id,
                        v.Name,
                        v.Email,
                        v.Address,
                        v.PhoneNumber,
                        (v.VendorGroup != null ? v.VendorGroup.Name : ""),
                        v.CreatedDate
                        )
                    )
                    .ToListAsync();
                return Results.Ok(new Response(true, vendors, ""));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

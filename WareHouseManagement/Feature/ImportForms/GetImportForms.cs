using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.ImportForm {
    public class GetImportForms : IEndpoint {
        public record ReceiptDTO(string Id, string VendorName, DateTime DateOfOrder);
        public record FormDTO(string Id, ReceiptDTO Receipt, DateTime DateOfImport);
        public record Response(bool Success, List<FormDTO> Data, string ErrorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Import-Forms", Handler).WithTags("Import Forms");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Stock)]
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == User.Identity.Name)
                    .Select(u => u.ServiceId)
                    .FirstOrDefaultAsync();

                var Forms = await context.ImportForms
                    .Include(form => form.Receipt)
                    .ThenInclude(receipt => receipt.Vendor)
                    .Where(form => form.ServiceId == ServiceId)
                    .Where(form=>!form.IsDeleted)
                    .OrderByDescending(form => form.CreatedDate)
                    .Select(form => new FormDTO(
                        form.Id,
                        new ReceiptDTO(
                            form.ReceiptId,
                            form.Receipt.Vendor.Name,
                            form.Receipt.DateOrder
                        ),
                        form.ImportDate
                        )
                    )
                    .ToListAsync();

                return Results.Ok(new Response(true, Forms, ""));
            }
            catch (Exception ex) {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

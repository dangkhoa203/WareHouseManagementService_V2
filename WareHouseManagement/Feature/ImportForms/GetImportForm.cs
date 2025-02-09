using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;

namespace WareHouseManagement.Feature.ImportForm {
    public class GetImportForm:IEndpoint {
        public record receiptDTO(string id, string vendorName, DateTime dateOfOrder);
        public record detailDTO(string productID, string productName,string warehouseId,string warehouseName,string address,string city, int quantity);
        public record formDTO(string id, receiptDTO receipt, ICollection<detailDTO> details, DateTime dateOfImport,DateTime createDate);
        public record Response(bool success, formDTO data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Import-Forms/{id}", Handler).RequireAuthorization().WithTags("Import Forms");
        }
        private static async Task<IResult> Handler(string id, ApplicationDbContext context, ClaimsPrincipal user) {
            try {
                var service = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceRegistered)
                    .FirstOrDefault();
                var form = await context.StockImportForms
                    .Include(f=>f.Details)
                       .ThenInclude(d=>d.ProductNav)
                    .Include(f => f.Details)
                       .ThenInclude(d => d.WarehouseNav)
                    .Include(f => f.Receipt)
                       .ThenInclude(re=>re.Vendor)
                    .Where(t => t.ServiceRegisteredFrom.Id == service.Id)
                    .Where(r => !r.IsDeleted)
                    .FirstOrDefaultAsync(r => r.Id == id);
                if (form != null) {
                    var receipt = new receiptDTO(
                        form.Receipt.Id,
                        form.Receipt.Vendor.Name,
                        form.Receipt.DateOrder
                    );
                    var details = form.Details
                    .Select(
                        d => new detailDTO(
                        d.ProductId,
                        d.ProductNav.Name,
                        d.WarehouseId,
                        d.WarehouseNav.Name,
                        d.WarehouseNav.Address,
                        d.WarehouseNav.City,
                        d.Quantity
                        )
                    )
                    .ToList();
                    var data = new formDTO(form.Id,receipt, details, form.ImportDate, form.CreatedDate);
                    return Results.Ok(new Response(true, data, ""));
                }
                return Results.NotFound(new Response(false, null, "Không tìm thấy dữ liệu!"));
            } catch (Exception ex) {
                return Results.BadRequest(new Response(false, null, "Lỗi đã xảy ra!"));
            }
        }
    }
}

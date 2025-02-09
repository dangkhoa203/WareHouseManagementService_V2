using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;

namespace WareHouseManagement.Feature.ExportForms {
    public class GetExportForm:IEndpoint {
        public record receiptDTO(string id, string customerName, DateTime dateOfOrder);
        public record detailDTO(string productID, string productName, string warehouseId, string warehouseName, string address, string city, int quantity);
        public record formDTO(string id, receiptDTO receipt, ICollection<detailDTO> details, DateTime dateOfExport, DateTime createDate);
        public record Response(bool success, formDTO data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Export-Forms/{id}", Handler).RequireAuthorization().WithTags("Export Forms");
        }
        private static async Task<IResult> Handler(string id, ApplicationDbContext context, ClaimsPrincipal user) {
            try {
                var service = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceRegistered)
                    .FirstOrDefault();
                var form = await context.StockExportForms
                    .Include(f => f.Details)
                       .ThenInclude(d => d.ProductNav)
                    .Include(f => f.Details)
                       .ThenInclude(d => d.WarehouseNav)
                    .Include(f => f.Receipt)
                       .ThenInclude(re => re.Customer)
                    .Where(t => t.ServiceRegisteredFrom.Id == service.Id)
                    .Where(r => !r.IsDeleted)
                    .FirstOrDefaultAsync(r => r.Id == id);
                if (form != null) {
                    var receipt = new receiptDTO(
                        form.Receipt.Id,
                        form.Receipt.Customer.Name,
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
                    var data = new formDTO(form.Id, receipt, details, form.ExportDate, form.CreatedDate);
                    return Results.Ok(new Response(true, data, ""));
                }
                return Results.NotFound(new Response(false, null, "Không tìm thấy dữ liệu!"));
            } catch (Exception ex) {
                return Results.BadRequest(new Response(false, null, "Lỗi đã xảy ra!"));
            }
        }
    }
}

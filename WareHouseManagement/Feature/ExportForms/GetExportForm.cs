using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.ExportForms {
    public class GetExportForm : IEndpoint {
        public record ReceiptDTO(string Id, string CustomerName, DateTime DateOfOrder);
        public record DetailDTO(string ProductID, string ProductName, string WarehouseId, string WarehouseName, string Address, string City, int Quantity);
        public record FormDTO(string Id, ReceiptDTO Receipt, ICollection<DetailDTO> Details, DateTime DateOfExport, DateTime DateCreated);
        public record Response(bool Success, FormDTO Data, string ErrorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Export-Forms/{id}", Handler).WithTags("Export Forms");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Stock)]
        private static async Task<IResult> Handler(string id, ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                   .Include(u => u.ServiceRegistered)
                   .Where(u => u.UserName == User.Identity.Name)
                   .Select(u => u.ServiceId)
                   .FirstOrDefaultAsync();

                var Form = await context.ExportForms
                    .Include(form => form.Details)
                       .ThenInclude(detail => detail.ProductNav)
                    .Include(form => form.Details)
                       .ThenInclude(detail => detail.WarehouseNav)
                    .Include(form => form.Receipt)
                       .ThenInclude(receipt => receipt.Customer)
                    .Where(u => u.ServiceId == ServiceId)
                    .FirstOrDefaultAsync(form => form.Id == id);

                if (Form == null) 
                    return Results.NotFound(new Response(false, null, "Không tìm thấy dữ liệu!"));
                if (Form.IsDeleted)
                    return Results.NotFound(new Response(false, null, "Dữ liệu đã xóa!"));

                var Receipt = new ReceiptDTO(
                     Form.Receipt.Id,
                     Form.Receipt.Customer.Name,
                     Form.Receipt.DateOrder
                );

                var Details = Form.Details
                .Select(
                    detail => new DetailDTO(
                    detail.ProductId,
                    detail.ProductNav.Name,
                    detail.WarehouseId,
                    detail.WarehouseNav.Name,
                    detail.WarehouseNav.Address,
                    detail.WarehouseNav.City,
                    detail.Quantity
                    )
                )
                .ToList();

                var Data = new FormDTO(Form.Id, Receipt, Details, Form.ExportDate, Form.CreatedDate);
                return Results.Ok(new Response(true, Data, ""));
            }
            catch (Exception ex) {
                return Results.BadRequest(new Response(false, null, "Lỗi đã xảy ra!"));
            }
        }
    }
}

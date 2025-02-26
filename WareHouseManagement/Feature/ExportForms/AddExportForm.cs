using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity;
using WareHouseManagement.Model.Enum;
using WareHouseManagement.Model.Form;

namespace WareHouseManagement.Feature.ExportForms {
    public class AddExportForm : IEndpoint {
        public record detailDTO(string productId, string warehouseId, int quantity);
        public record Request(string receiptId, DateTime dateOfExport, List<detailDTO> details, bool updateStock);
        public record Response(bool success, string errorMessage, ValidationResult? error);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {

            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Export-Forms", Handler).WithTags("Export Forms");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Stock)]
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, ClaimsPrincipal user) {
            var validator = new Validator();
            var validatedresult = validator.Validate(request);
            if (!validatedresult.IsValid) {
                return Results.BadRequest(new Response(false, "", validatedresult));
            }
            var serviceId = context.Users.Include(u => u.ServiceRegistered).Where(u => u.UserName == user.Identity.Name).Select(u => u.ServiceId).FirstOrDefault();
            var receipt = await context.CustomerBuyReceipts.Include(re => re.Details).FirstOrDefaultAsync(re => re.Id == request.receiptId);
            if (receipt == null)
                return Results.BadRequest(new Response(false, "không tìm thấy hóa đơn!", validatedresult));

            var details = new List<ExportFormDetail>();
            foreach (var detail in request.details) {
                var product = await context.Products.FindAsync(detail.productId);
                var warehouse = await context.Warehouses.FindAsync(detail.warehouseId);

                if (product == null || warehouse == null)
                    return Results.BadRequest(new Response(false, "không tìm thấy dữ liệu!", validatedresult));

                if (request.updateStock) {
                    var stockCount = await context.Stocks
                        .Where(s => s.WarehouseId == detail.warehouseId && s.ProductId == detail.productId)
                        .Select(s => s.Quantity)
                        .FirstOrDefaultAsync();
                    if (stockCount == null || detail.quantity > stockCount)
                        return Results.BadRequest(new Response(false, "Không đủ số lượng để xuất kho!", validatedresult));
                }

                details.Add(new ExportFormDetail() {
                    ProductNav = product,
                    WarehouseNav = warehouse,
                    Quantity = detail.quantity,
                });
            }
            foreach (var de in receipt.Details) {
                int check = details.Where(fd => fd.ProductNav.Id == de.ProductId).Sum(d => d.Quantity);
                if (check != de.Quantity)
                    return Results.BadRequest(new Response(false, "Không đủ số lượng chưa chính xác!", validatedresult));
            }
            var form = new StockExportForm() {
                ReceiptId = request.receiptId,
                ExportDate = request.dateOfExport,
                Details = details,
                ServiceId = serviceId,
            };
            await context.StockExportForms.AddAsync(form);

            if (request.updateStock)
                await exportStock(details, context, serviceId);

            if (await context.SaveChangesAsync() > 0) {
                return Results.Ok(new Response(true, "", validatedresult));
            }
            return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", validatedresult));
        }
        private static async Task exportStock(List<ExportFormDetail> details, ApplicationDbContext context, string serviceId) {
            foreach (var de in details) {
                var stock = await context.Stocks.Where(s => s.WarehouseId == de.WarehouseId).FirstOrDefaultAsync(s => s.ProductId == de.ProductId);
                stock.Quantity -= de.Quantity;
            }
        }
    }
}

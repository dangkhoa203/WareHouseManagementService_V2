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
using WareHouseManagement.Model.Receipt;

namespace WareHouseManagement.Feature.ImportForm {
    public class AddImportForm : IEndpoint {
        public record detailDTO(string productId, string warehouseId, int quantity);
        public record Request(string receiptId, DateTime dateOfImport, List<detailDTO> details, bool updateStock);
        public record Response(bool success, string errorMessage, ValidationResult? error);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {

            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Import-Forms", Handler).WithTags("Import Forms");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Stock)]
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, ClaimsPrincipal user) {
            var validator = new Validator();
            var validatedresult = validator.Validate(request);
            if (!validatedresult.IsValid) {
                return Results.BadRequest(new Response(false, "", validatedresult));
            }
            var service = context.Users.Include(u => u.ServiceRegistered).Where(u => u.UserName == user.Identity.Name).Select(u => u.ServiceRegistered).FirstOrDefault();
            var receipt = await context.VendorReplenishReceipts.Include(re => re.Details).FirstOrDefaultAsync(re => re.Id == request.receiptId);
            if (receipt == null)
                return Results.BadRequest(new Response(false, "không tìm thấy hóa đơn!", validatedresult));

            var details = new List<ImportFormDetail>();
            foreach (var detail in request.details) {
                var product = await context.Products.FindAsync(detail.productId);
                var warehouse = await context.Warehouses.FindAsync(detail.warehouseId);
                if (product == null || warehouse == null)
                    return Results.BadRequest(new Response(false, "không tìm thấy dữ liệu!", validatedresult));
                details.Add(new ImportFormDetail() {
                    ProductNav = product,
                    WarehouseNav = warehouse,
                    Quantity = detail.quantity,
                });
            }
            foreach (var de in receipt.Details) {
                int check = details.Where(fd => fd.ProductNav.Id == de.ProductId).Sum(d => d.Quantity);
                if (check != de.Quantity)
                    return Results.BadRequest(new Response(false, "Không đủ số lượng để tồn kho!", validatedresult));
            }
            var form = new StockImportForm() {
                ReceiptId = request.receiptId,
                ImportDate = request.dateOfImport,
                Details = details,
                ServiceRegisteredFrom = service,
            };
            await context.StockImportForms.AddAsync(form);

            if (request.updateStock)
                await importStock(details, context, service);

            if (await context.SaveChangesAsync() > 0) {
                return Results.Ok(new Response(true, "", validatedresult));
            }
            return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", validatedresult));
        }
        private static async Task importStock(List<ImportFormDetail> details, ApplicationDbContext context, ServiceRegistered service) {
            foreach (var de in details) {
                var stock = await context.Stocks.Where(s => s.WarehouseId == de.WarehouseId).FirstOrDefaultAsync(s => s.ProductId == de.ProductId);
                if (stock != null) {
                    stock.Quantity += de.Quantity;
                }
                else {
                    var newstock = new Stock() {
                        ProductId = de.ProductId,
                        WarehouseId = de.WarehouseId,
                        Quantity = de.Quantity,
                        ServiceRegisteredFrom = service,
                    };
                    await context.Stocks.AddAsync(newstock);
                }
            }
        }
    }
}

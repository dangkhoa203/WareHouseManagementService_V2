using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity;
using WareHouseManagement.Model.Entity.Warehouse_Entity;
using WareHouseManagement.Model.Enum;
using WareHouseManagement.Model.Form;
using WareHouseManagement.Model.Receipt;

namespace WareHouseManagement.Feature.ImportForm {
    public class AddImportForm : IEndpoint {
        public record DetailDTO(string ProductId, string WarehouseId, int Quantity);
        public record Request(string ReceiptId, DateTime DateOfImport, List<DetailDTO> Details, bool UpdateStock);
        public record Response(bool Success, string ErrorMessage, ValidationResult? ValidateError);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {

            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Import-Forms", Handler).WithTags("Import Forms");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Stock)]
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var Validator = new Validator();
                var ValidatedResult = Validator.Validate(request);
                if (!ValidatedResult.IsValid) {
                    return Results.BadRequest(new Response(false, "", ValidatedResult));
                }

                var ServiceId = await context.Users
                       .Include(u => u.ServiceRegistered)
                       .Where(u => u.UserName == User.Identity.Name)
                       .Select(u => u.ServiceId)
                       .FirstOrDefaultAsync();

                var Receipt = await context.VendorReplenishReceipts.Include(receipt => receipt.Details).FirstOrDefaultAsync(receipt => receipt.Id == request.ReceiptId);
                if (Receipt == null)
                    return Results.BadRequest(new Response(false, "không tìm thấy hóa đơn!", ValidatedResult));

                var Details = new List<ImportFormDetail>();
                foreach (var FormDetail in request.Details) {
                    var Product = await context.Products.FindAsync(FormDetail.ProductId);
                    var WareHouse = await context.Warehouses.FindAsync(FormDetail.WarehouseId);

                    if (Product == null || WareHouse == null)
                        return Results.BadRequest(new Response(false, "không tìm thấy dữ liệu!", ValidatedResult));

                    Details.Add(new ImportFormDetail() {
                        ProductNav = Product,
                        WarehouseNav = WareHouse,
                        Quantity = FormDetail.Quantity,
                    });
                }

                foreach (var ReceiptDetail in Receipt.Details) {
                    int check = Details.Where(FormDetail => FormDetail.ProductNav.Id == ReceiptDetail.ProductId).Sum(FormDetail => FormDetail.Quantity);
                    if (check != ReceiptDetail.Quantity)
                        return Results.BadRequest(new Response(false, "Không đủ số lượng để tồn kho!", ValidatedResult));
                }

                var form = new Model.Form.ImportForm() {
                    ReceiptId = request.ReceiptId,
                    ImportDate = request.DateOfImport,
                    Details = Details,
                    ServiceId = ServiceId,
                };
                await context.ImportForms.AddAsync(form);

                if (request.UpdateStock)
                    await importStock(Details, context, ServiceId);

                if (await context.SaveChangesAsync() > 0) {
                    return Results.Ok(new Response(true, "", ValidatedResult));
                }

                return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", ValidatedResult));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra!", null));
            }
        }
        private static async Task importStock(List<ImportFormDetail> Details, ApplicationDbContext context, string ServiceId) {
            foreach (var FormDetail in Details) {
                var stock = await context.Stocks.Where(s => s.WarehouseId == FormDetail.WarehouseId).FirstOrDefaultAsync(s => s.ProductId == FormDetail.ProductId);
                if (stock != null) {
                    stock.Quantity += FormDetail.Quantity;
                }
                else {
                    var newstock = new Stock() {
                        ProductId = FormDetail.ProductId,
                        WarehouseId = FormDetail.WarehouseId,
                        Quantity = FormDetail.Quantity,
                        ServiceId = ServiceId,
                    };
                    await context.Stocks.AddAsync(newstock);
                }
            }
        }
    }
}

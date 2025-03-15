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
        public record DetailDTO(string ProductId, string WarehouseId, int Quantity);
        public record Request(string ReceiptId, DateTime DateOfExport, List<DetailDTO> Details, bool UpdateStock);
        public record Response(bool Success, string ErrorMessage, ValidationResult? ValidateError);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {

            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Export-Forms", Handler).WithTags("Export Forms");
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

                var Receipt = await context.CustomerBuyReceipts.Include(receipt => receipt.Details).FirstOrDefaultAsync(receipt => receipt.Id == request.ReceiptId);
                if (Receipt == null)
                    return Results.BadRequest(new Response(false, "không tìm thấy hóa đơn!", ValidatedResult));

                var Details = new List<ExportFormDetail>();
                foreach (var FormDetail in request.Details) {
                    var Product = await context.Products.FindAsync(FormDetail.ProductId);
                    var Warehouse = await context.Warehouses.FindAsync(FormDetail.WarehouseId);

                    if (Product == null || Warehouse == null)
                        return Results.BadRequest(new Response(false, "không tìm thấy dữ liệu!", ValidatedResult));

                    if (request.UpdateStock) {
                        var StockCount = await context.Stocks
                            .Where(s => s.WarehouseId == FormDetail.WarehouseId && s.ProductId == FormDetail.ProductId)
                            .Select(s => s.Quantity)
                            .FirstOrDefaultAsync();
                        if (StockCount == null || FormDetail.Quantity > StockCount)
                            return Results.BadRequest(new Response(false, "Không đủ số lượng để xuất kho!", ValidatedResult));
                    }

                    Details.Add(new ExportFormDetail() {
                        ProductNav = Product,
                        WarehouseNav = Warehouse,
                        Quantity = FormDetail.Quantity,
                    });
                }

                foreach (var ReceiptDetail in Receipt.Details) {
                    int check = Details.Where(FormDetail => FormDetail.ProductNav.Id == ReceiptDetail.ProductId).Sum(FormDetail => FormDetail.Quantity);
                    if (check != ReceiptDetail.Quantity)
                        return Results.BadRequest(new Response(false, "Không đủ số lượng chưa chính xác!", ValidatedResult));
                }

                var ExportForm = new ExportForm() {
                    ReceiptId = request.ReceiptId,
                    ExportDate = request.DateOfExport,
                    Details = Details,
                    ServiceId = ServiceId,
                };
                await context.ExportForms.AddAsync(ExportForm);

                if (request.UpdateStock)
                    await ExportStock(Details, context, ServiceId);

                if (await context.SaveChangesAsync() > 0) {
                    return Results.Ok(new Response(true, "", ValidatedResult));
                }
                return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", ValidatedResult));
            }
            catch (Exception)
            {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra!", null));
            }
        }
        private static async Task ExportStock(List<ExportFormDetail> Details, ApplicationDbContext context, string ServiceId) {
            foreach (var FormDetail in Details) {
                var stock = await context.Stocks.Where(s => s.WarehouseId == FormDetail.WarehouseId).FirstOrDefaultAsync(s => s.ProductId == FormDetail.ProductId);
                stock.Quantity -= FormDetail.Quantity;
            }
        }
    }
}

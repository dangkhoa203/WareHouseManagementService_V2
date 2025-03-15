using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;
using WareHouseManagement.Model.Receipt;

namespace WareHouseManagement.Feature.VendorReplenishReceipts {
    public class AddVendorReceipt:IEndpoint {
        public record DetailDTO(string ProductId, int Quantity);
        public record Request(string VendorId, string TaxId, DateTime dateOfOrder, List<DetailDTO> Details);
        public record Response(bool Success, string ErrorMessage, ValidationResult? ValidateError);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {

            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Vendor-Receipts", Handler).WithTags("Vendor Receipts");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.VendorReceipt)]
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

                var Details = new List<VendorReplenishReceiptDetail>();
                foreach (var re in request.Details) {
                    var NewDetail = new VendorReplenishReceiptDetail();
                    NewDetail.ProductNav = await context.Products.FindAsync(re.ProductId);
                    NewDetail.Quantity = re.Quantity;
                    NewDetail.PriceOfOne = NewDetail.ProductNav.PricePerUnit;
                    NewDetail.TotalPrice = NewDetail.PriceOfOne * NewDetail.Quantity;
                    Details.Add(NewDetail);
                }

                var Receipt = new VendorReplenishReceipt() {
                    Vendor = await context.Vendors.FindAsync(request.VendorId),
                    DateOrder = request.dateOfOrder,
                    Tax = await context.Taxes.FindAsync(request.TaxId),
                    ReceiptValue = Details.Sum(d => d.TotalPrice),
                    Details = Details,
                    ServiceId = ServiceId,
                };
                await context.VendorReplenishReceipts.AddAsync(Receipt);
                if (await context.SaveChangesAsync() > 0) {
                    return Results.Ok(new Response(true, "", ValidatedResult));
                }

                return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", ValidatedResult));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra!", null));
            }
        }
    }
}

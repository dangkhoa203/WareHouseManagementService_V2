using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Receipt;

namespace WareHouseManagement.Feature.VendorReplenishReceipts {
    public class AddVendorReceipt:IEndpoint {
        public record detailDTO(string productId, int quantity);
        public record Request(string vendorId, DateTime dateOfOrder, List<detailDTO> details);
        public record Response(bool success, string errorMessage, ValidationResult? error);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {

            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Vendor-Receipts", Handler).RequireAuthorization().WithTags("Vendor Receipts");
        }
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, ClaimsPrincipal user) {
            var validator = new Validator();
            var validatedresult = validator.Validate(request);
            if (!validatedresult.IsValid) {
                return Results.BadRequest(new Response(false, "", validatedresult));
            }
            var service = context.Users.Include(u => u.ServiceRegistered).Where(u => u.UserName == user.Identity.Name).Select(u => u.ServiceRegistered).FirstOrDefault();
            var details = new List<VendorReplenishReceiptDetail>();
            foreach (var re in request.details) {
                var newdetail = new VendorReplenishReceiptDetail();
                newdetail.ProductNav = await context.Products.FindAsync(re.productId);
                newdetail.Quantity = re.quantity;
                newdetail.PriceOfOne = newdetail.ProductNav.PricePerUnit;
                newdetail.TotalPrice = newdetail.PriceOfOne * newdetail.Quantity;
                details.Add(newdetail);
            }
            var receipt = new VendorReplenishReceipt() {
                Vendor = await context.Vendors.FindAsync(request.vendorId),
                DateOrder = request.dateOfOrder,
                Details = details,
                ServiceRegisteredFrom = service,
            };
            await context.VendorReplenishReceipts.AddAsync(receipt);
            if (await context.SaveChangesAsync() > 0) {
                return Results.Ok(new Response(true, "", validatedresult));
            }
            return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", validatedresult));
        }
    }
}

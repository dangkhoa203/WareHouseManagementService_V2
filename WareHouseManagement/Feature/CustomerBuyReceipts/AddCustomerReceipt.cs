using System.Security.Claims;
using FluentValidation.Results;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity;
using WareHouseManagement.Model.Receipt;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.CustomerBuyReceipts {
    public class AddCustomerReceipt : IEndpoint {
        public record DetailDTO(string productId, int quantity);
        public record Request(string CustomerId, string TaxId, DateTime DateOfOrder, List<DetailDTO> Details);
        public record Response(bool Success, string ErrorMessage, ValidationResult? ValidateError);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {

            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Customer-Receipts", Handler).WithTags("Customer Receipts");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.CustomerReceipt)]
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

                var Details = new List<CustomerBuyReceiptDetail>();
                foreach (var re in request.Details) {
                    var NewDetail = new CustomerBuyReceiptDetail();
                    NewDetail.ProductNav = await context.Products.FindAsync(re.productId);
                    NewDetail.Quantity = re.quantity;
                    NewDetail.PriceOfOne = NewDetail.ProductNav.PricePerUnit;
                    NewDetail.TotalPrice = NewDetail.PriceOfOne * NewDetail.Quantity;
                    Details.Add(NewDetail);
                }

                var Receipt = new CustomerBuyReceipt() {
                    Customer = await context.Customers.FindAsync(request.CustomerId),
                    DateOrder = request.DateOfOrder,
                    Tax = await context.Taxes.FindAsync(request.TaxId),
                    ReceiptValue = Details.Sum(d => d.TotalPrice),
                    Details = Details,
                    ServiceId = ServiceId,
                };
                await context.CustomerBuyReceipts.AddAsync(Receipt);

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

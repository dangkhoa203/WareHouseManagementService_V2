using System.Security.Claims;
using FluentValidation.Results;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using static System.Runtime.InteropServices.JavaScript.JSType;
using WareHouseManagement.Model.Entity;
using WareHouseManagement.Model.Receipt;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace WareHouseManagement.Feature.CustomerBuyReceipts {
    public class AddCustomerReceipt : IEndpoint {
        public record detailDTO(string productId,int quantity);
        public record Request(string customerId,DateTime dateOfOrder,List<detailDTO> details);
        public record Response(bool success, string errorMessage, ValidationResult? error);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
              
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Customer-Receipts", Handler).WithTags("CustomerReceipts");
        }
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, ClaimsPrincipal user) {
            var validator = new Validator();
            var validatedresult = validator.Validate(request);
            if (!validatedresult.IsValid) {
                return Results.BadRequest(new Response(false, "", validatedresult));
            }
            var service = context.Users.Include(u => u.ServiceRegistered).Where(u => u.UserName == user.Identity.Name).Select(u => u.ServiceRegistered).FirstOrDefault();
            var detail = new List<CustomerBuyReceiptDetail>();
            foreach (var re in request.details) {
                var newdetail = new CustomerBuyReceiptDetail();
                newdetail.ProductNav = await context.Products.FindAsync(re.productId);
                newdetail.Quantity = re.quantity;
                newdetail.PriceOfOne = newdetail.ProductNav.PricePerUnit;
                newdetail.TotalPrice= newdetail.PriceOfOne * newdetail.Quantity;
                detail.Add(newdetail);
            }
            var receipt = new CustomerBuyReceipt() {
                Customer = await context.Customers.FindAsync(request.customerId),
                DateOrder = request.dateOfOrder,
                Details = detail,
                ServiceRegisteredFrom = service,
            };
            await context.CustomerBuyReceipts.AddAsync(receipt);
            if (await context.SaveChangesAsync() > 0) {
                return Results.Ok(new Response(true, "", validatedresult));
            }
            return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", validatedresult));
        }
    }
}

using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;

namespace WareHouseManagement.Feature.CustomerBuyReceipts {
    public class GetCustomerReceipt : IEndpoint {
        public record customerDTO(string id, string name, string email, string address, string phoneNumber, string groupName);
        public record detailDTO(string productID, string productName, int price, int quantity, int totalPrice);
        public record receiptDTO(customerDTO customer, ICollection<detailDTO> details, DateTime dateOfOrder, DateTime createDate);
        public record Response(bool success, receiptDTO data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Customer-Receipts/{id}", Handler).RequireAuthorization().WithTags("Customer Receipts");
        }
        private static async Task<IResult> Handler(string id, ApplicationDbContext context, ClaimsPrincipal user) {
            try {
                var service = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceRegistered)
                    .FirstOrDefault();
                var Receipt = await context.CustomerBuyReceipts
                    .Include(r => r.Details)
                    .ThenInclude(d => d.ProductNav)
                    .Include(r => r.Customer)
                    .ThenInclude(c => c.CustomerGroup)
                    .Where(t => t.ServiceRegisteredFrom.Id == service.Id)
                    .Where(r => !r.IsDeleted)
                    .FirstOrDefaultAsync(r => r.Id == id);
                if (Receipt != null) {
                    var customer = new customerDTO(
                        Receipt.Customer.Id,
                        Receipt.Customer.Name,
                        Receipt.Customer.Email,
                        Receipt.Customer.Address,
                        Receipt.Customer.PhoneNumber,
                        Receipt.Customer.CustomerGroup.Name
                    );
                    var details = Receipt.Details
                    .Select(
                        d => new detailDTO(
                        d.ProductId,
                        d.ProductNav.Name,
                        d.PriceOfOne,
                        d.Quantity,
                        d.TotalPrice
                        )
                    )
                    .ToList();
                    var receipt = new receiptDTO(customer,details, Receipt.DateOrder, Receipt.CreatedDate);
                    return Results.Ok(new Response(true, receipt, ""));
                }
                return Results.NotFound(new Response(false, null, "Không tìm thấy dữ liệu!"));
            } catch (Exception ex) {
                return Results.BadRequest(new Response(false, null, "Lỗi đã xảy ra!"));
            }
        }
    }
}

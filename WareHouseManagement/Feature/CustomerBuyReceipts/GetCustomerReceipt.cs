using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.CustomerBuyReceipts {
    public class GetCustomerReceipt : IEndpoint {
        public record CustomerDTO(string Id, string Name, string Email, string Address, string PhoneNumber, string GroupName);
        public record TaxDTO(string Id, string Description, float percent);
        public record DetailDTO(string ProductID, string ProductName, float Price, int Quantity, float TotalPrice);
        public record ReceiptDTO(CustomerDTO Customer, ICollection<DetailDTO> Details, TaxDTO? Tax, DateTime DateOfOrder, DateTime DateCreated);
        public record Response(bool Success, ReceiptDTO? Data, string ErrorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Customer-Receipts/{id}", Handler).WithTags("Customer Receipts");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.CustomerReceipt)]
        private static async Task<IResult> Handler(string id, ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                                .Include(u => u.ServiceRegistered)
                                .Where(u => u.UserName == User.Identity.Name)
                                .Select(u => u.ServiceId)
                                .FirstOrDefaultAsync();

                var Receipt = await context.CustomerBuyReceipts
                    .Include(receipt => receipt.Tax)
                    .Include(receipt => receipt.Details)
                       .ThenInclude(detail => detail.ProductNav)
                    .Include(receipt => receipt.Customer)
                       .ThenInclude(c => c.CustomerGroup)
                    .Where(receipt => receipt.ServiceId == ServiceId)
                    .FirstOrDefaultAsync(receipt => receipt.Id == id);

                if (Receipt == null) 
                    return Results.NotFound(new Response(false, null, "Không tìm thấy dữ liệu!"));
                if (Receipt.IsDeleted) 
                    return Results.NotFound(new Response(false, null, "Dữ liệu đã xóa!"));


                var Customer = new CustomerDTO(
                        Receipt.Customer.Id,
                        Receipt.Customer.Name,
                        Receipt.Customer.Email,
                        Receipt.Customer.Address,
                        Receipt.Customer.PhoneNumber,
                        Receipt.Customer.CustomerGroup.Name
                );

                var Details = Receipt.Details
                .Select(
                    detail => new DetailDTO(
                        detail.ProductId,
                        detail.ProductNav.Name,
                        detail.PriceOfOne,
                        detail.Quantity,
                        detail.TotalPrice
                    )
                ).ToList();

                var Tax = Receipt.Tax != null ? new TaxDTO(Receipt.Tax.Id, Receipt.Tax.Name, Receipt.Tax.Percent) : null;
                var Data = new ReceiptDTO(Customer, Details, Tax, Receipt.DateOrder, Receipt.CreatedDate);
                return Results.Ok(new Response(true, Data, ""));
            }
            catch (Exception ex) {
                return Results.BadRequest(new Response(false, null, "Lỗi đã xảy ra!"));
            }
        }
    }
}

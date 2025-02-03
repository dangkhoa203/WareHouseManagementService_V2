using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;

namespace WareHouseManagement.Feature.CustomerBuyReceipts {
    public class GetCustomerReceipts : IEndpoint {
        public record customerDTO(string id, string name, string email, string address, string phoneNumber);
        public record receiptDTO(string id, customerDTO customer, DateTime dateOfOrder);
        public record Response(bool success, List<receiptDTO> data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Customer-Receipts", Handler).WithTags("CustomerReceipts");
        }
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal user) {
            try {
                var service = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceRegistered)
                    .FirstOrDefault();
                var receipts = await context.CustomerBuyReceipts
                    .Include(re => re.Customer)
                    .Where(re => re.ServiceRegisteredFrom.Id == service.Id)
                    .OrderByDescending(re => re.CreatedDate)
                    .Select(re => new receiptDTO(
                        re.Id,
                        new customerDTO(
                            re.Customer.Id,
                            re.Customer.Name,
                            re.Customer.Email,
                            re.Customer.Address,
                            re.Customer.PhoneNumber
                            ),
                        re.DateOrder
                    ))
                    .ToListAsync();
                return Results.Ok(new Response(true, receipts, ""));
            } catch (Exception ex) {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

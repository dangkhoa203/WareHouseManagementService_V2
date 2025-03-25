using System.Security.Claims;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.CustomerBuyReceipts {
    public class UpdateCustomerReceiptStatus : IEndpoint {
        public record Request(string Id, StatusEnum Status);
        public record Response(bool Success, string ErrorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPut("/api/Customer-Receipts", Handler).WithTags("Customer Receipts");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.CustomerReceipt)]
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                                    .Include(u => u.ServiceRegistered)
                                    .Where(u => u.UserName == User.Identity.Name)
                                    .Select(u => u.ServiceId)
                                    .FirstOrDefaultAsync();

                var Receipt = await context.CustomerBuyReceipts
                    .Where(receipt => receipt.ServiceId == ServiceId)
                    .Where(receipt => !receipt.IsDeleted)
                    .FirstOrDefaultAsync(u => u.Id == request.Id);

                if (Receipt == null)
                    return Results.NotFound(new Response(false, "Lỗi xảy ra khi đang thực hiện!"));

                if (request.Status != Receipt.Status) {
                    Receipt.Status = request.Status;
                    if (await context.SaveChangesAsync() < 1) {
                        return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!"));
                    }
                }
                return Results.Ok(new Response(true, ""));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra!"));
            }

        }
    }
}

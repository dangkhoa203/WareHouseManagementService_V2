using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;

namespace WareHouseManagement.Feature.ExportForms {
    public class GetExportForms:IEndpoint {
        public record receiptDTO(string id, string customerName, DateTime dateOfOrder);
        public record formDTO(string id, receiptDTO receipt, DateTime dateOfExport);
        public record Response(bool success, List<formDTO> data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Export-Forms", Handler).RequireAuthorization().WithTags("Import Forms");
        }
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal user) {
            try {
                var service = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceRegistered)
                    .FirstOrDefault();
                var forms = await context.StockExportForms
                    .Include(f => f.Receipt)
                    .ThenInclude(re => re.Customer)
                    .Where(f => f.ServiceRegisteredFrom.Id == service.Id)
                    .OrderByDescending(f => f.CreatedDate)
                    .Select(f => new formDTO(
                        f.Id,
                        new receiptDTO(
                            f.ReceiptId,
                            f.Receipt.Customer.Name,
                            f.Receipt.DateOrder
                            ),
                        f.ExportDate
                    ))
                    .ToListAsync();
                return Results.Ok(new Response(true, forms, ""));
            } catch (Exception ex) {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;
using WareHouseManagement.Model.Receipt;

namespace WareHouseManagement.Feature.Taxes {
    public class GetTax : IEndpoint {
        public record TaxDTO(string Id, string Name, string Description, float Percent, DateTime DateCreated);
        public record Response(bool Success, TaxDTO Data, string ErrorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Taxes/{id}", Handler).WithTags("Taxes");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Tax)]
        private static async Task<IResult> Handler(string id, ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                   .Include(u => u.ServiceRegistered)
                   .Where(u => u.UserName == User.Identity.Name)
                   .Select(u => u.ServiceId)
                   .FirstOrDefaultAsync();

                var Tax = await context.Taxes
                    .Where(tax => tax.ServiceId == ServiceId)
                    .FirstOrDefaultAsync(tax => tax.Id == id);

                if (Tax == null)
                    return Results.NotFound(new Response(false, null, "Không tìm thấy dữ liệu!"));
                if (Tax.IsDeleted)
                    return Results.NotFound(new Response(false, null, "Dữ liệu đã xóa!"));

                var Data = new TaxDTO(Tax.Id, Tax.Name, Tax.Description, Tax.Percent, Tax.CreatedDate);
                return Results.Ok(new Response(true, Data, ""));
            }
            catch (Exception ex) {
                return Results.BadRequest(new Response(false, null, "Lỗi đã xảy ra!"));
            }
        }
    }
}

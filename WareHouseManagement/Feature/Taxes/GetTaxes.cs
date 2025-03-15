using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Taxes {
    public class GetTaxes : IEndpoint {
        public record TaxDTO(string Id, string Name, float Percent, DateTime DateCreated);
        public record Response(bool Success, List<TaxDTO> Data, string ErrorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Taxes", Handler).WithTags("Taxes");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Tax)]
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                   .Include(u => u.ServiceRegistered)
                   .Where(u => u.UserName == User.Identity.Name)
                   .Select(u => u.ServiceId)
                   .FirstOrDefaultAsync();

                var Taxes = await context.Taxes
                    .Where(tax => tax.ServiceId == ServiceId)
                    .Where(tax=>!tax.IsDeleted)
                    .OrderByDescending(tax => tax.CreatedDate)
                    .Select(tax => new TaxDTO(tax.Id, tax.Name, tax.Percent, tax.CreatedDate))
                    .ToListAsync();

                return Results.Ok(new Response(true, Taxes, ""));
            }
            catch (Exception ex) {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

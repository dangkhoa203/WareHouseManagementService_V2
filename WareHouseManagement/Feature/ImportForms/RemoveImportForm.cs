using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.ImportForm {
    public class RemoveImportForm : IEndpoint {
        public record Request(string Id);
        public record Response(bool Success, string ErrorMessage);
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapDelete("/api/Import-Forms/", Handler).WithTags("Import Forms");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Stock)]
        private static async Task<IResult> Handler([FromBody] Request request, ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                       .Include(u => u.ServiceRegistered)
                       .Where(u => u.UserName == User.Identity.Name)
                       .Select(u => u.ServiceId)
                       .FirstOrDefaultAsync();

                var Form = await context.ImportForms
                    .Where(form => form.ServiceId == ServiceId)
                    .FirstOrDefaultAsync(form => form.Id == request.Id);

                if (Form != null) {
                    Form.IsDeleted = true;
                    Form.DeletedAt = DateTime.Now;
                    var Result = await context.SaveChangesAsync();
                    if (Result > 0)
                        return Results.Ok(new Response(true, ""));
                    return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!"));
                }

                return Results.NotFound(new Response(false, "Không tìm thấy nhóm!"));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra!"));
            }
        }
    }
}

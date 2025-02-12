using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Customers
{
    public class GetCustomer:IEndpoint
    {
        public record groupDTO(string id,string name,string description);
        public record customerDTO(string id, string name, string email, string address, string phoneNumber, groupDTO? group, DateTime createDate);
        public record Response(bool success, customerDTO data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/Customers/{id}", Handler).WithTags("Customers");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Customer)]
        private static async Task<IResult> Handler([FromRoute]string id,ApplicationDbContext context, ClaimsPrincipal user)
        {
            try
            {
                var service = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceRegistered)
                    .FirstOrDefault();
                var customer = await context.Customers
                    .Include(c => c.CustomerGroup)
                    .Where(c => c.ServiceRegisteredFrom.Id == service.Id)
                    .Where(c => c.Id == id)
                    .Select(c => new customerDTO(

                        c.Id,
                        c.Name,
                        c.Email,
                        c.Address,
                        c.PhoneNumber,
                        c.CustomerGroup != null ?
                        new groupDTO(c.CustomerGroup.Id, c.CustomerGroup.Name, c.CustomerGroup.Description) : null,
                        c.CreatedDate

                     ))
                    .FirstOrDefaultAsync();
                if (customer != null)
                    return Results.Ok(new Response(true, customer, ""));
                return Results.NotFound(new Response(false, null, "Không tìm thấy dữ liệu!"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new Response(false, null, "Lỗi đã xảy ra!"));
            }
        }
    }
}

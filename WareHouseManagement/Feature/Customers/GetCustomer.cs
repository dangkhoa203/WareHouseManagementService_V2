using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Customer_Entity;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Customers {
    public class GetCustomer : IEndpoint {
        public record GroupDTO(string Id, string Name, string Description);
        public record CustomerDTO(string Id, string Name, string Email, string Address, string PhoneNumber, GroupDTO? Group, DateTime DateCreated);
        public record Response(bool Success, CustomerDTO Data, string ErrorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Customers/{id}", Handler).WithTags("Customers");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Customer)]
        private static async Task<IResult> Handler([FromRoute] string id, ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var ServiceId = await context.Users
                   .Include(u => u.ServiceRegistered)
                   .Where(u => u.UserName == User.Identity.Name)
                   .Select(u => u.ServiceId)
                   .FirstOrDefaultAsync();

                var Customer = await context.Customers
                    .Include(customer => customer.CustomerGroup)
                    .Where(customer => customer.ServiceId == ServiceId)
                    .FirstOrDefaultAsync(customer => customer.Id == id);

                if (Customer == null)
                    return Results.NotFound(new Response(false, null, "Không tìm thấy dữ liệu!"));
                if (Customer.IsDeleted) 
                    return Results.NotFound(new Response(false, null, "Dữ liệu đã xóa!"));


                var Data = new CustomerDTO(
                        Customer.Id,
                        Customer.Name,
                        Customer.Email,
                        Customer.Address,
                        Customer.PhoneNumber,
                        Customer.CustomerGroup != null ? new GroupDTO(Customer.CustomerGroup.Id, Customer.CustomerGroup.Name, Customer.CustomerGroup.Description) : null,
                        Customer.CreatedDate
                );
               
                return Results.Ok(new Response(true, Data, ""));
                
            }
            catch (Exception ex) {
                return Results.BadRequest(new Response(false, null, "Lỗi đã xảy ra!"));
            }
        }
    }
}

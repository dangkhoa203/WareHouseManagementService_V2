using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Customer_Entity;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Customers {
    public class UpdateCustomer : IEndpoint {
        public record Request(string Id, string Name, string Address, string Email, string Phone, string? GroupId);
        public record Response(bool Success, string ErrorMessage, ValidationResult? ValidateError);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.Name).NotEmpty().WithMessage("Chưa nhập tên");
            }
            private record Checkmodel(string Name, string Address, string Email, string Phone, string? GroupId);
            public bool checkSame(Request request, Customer customer) {
                Checkmodel newDetail = new (request.Name, request.Address, request.Email, request.Phone, request.GroupId);
                Checkmodel oldDetail = new (customer.Name, customer.Address, customer.Email, customer.PhoneNumber, customer.CustomerGroup != null ? customer.CustomerGroup.Id : "");
                return oldDetail == newDetail;

            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPut("/api/Customers", Handler).WithTags("Customers");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Customer)]
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, ClaimsPrincipal User) {
            try {
                var Validator = new Validator();
                var ValidatedResult = Validator.Validate(request);
                if (!ValidatedResult.IsValid) {
                    return Results.BadRequest(new Response(false, "", ValidatedResult));
                }

                var ServiceId = await context.Users
                       .Include(u => u.ServiceRegistered)
                       .Where(u => u.UserName == User.Identity.Name)
                       .Select(u => u.ServiceId)
                       .FirstOrDefaultAsync();

                var Customer = await context.Customers
                    .Include(customer => customer.CustomerGroup)
                    .Where(customer => customer.ServiceId == ServiceId)
                    .FirstOrDefaultAsync(customer => customer.Id == request.Id);
                if (Customer == null)
                    return Results.NotFound(new Response(false, "Lỗi xảy ra khi đang thực hiện!", ValidatedResult));

                if (!Validator.checkSame(request, Customer)) {
                    Customer.Name = request.Name;
                    Customer.Email = request.Email;
                    Customer.PhoneNumber = request.Phone;
                    Customer.Address = request.Address;
                    Customer.CustomerGroup = await context.CustomerGroups.FindAsync(request.GroupId);
                    if (await context.SaveChangesAsync() < 1) {
                        return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", ValidatedResult));
                    }
                }
                return Results.Ok(new Response(true, "", ValidatedResult));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra!", null));
            }
           
        }
    }
}

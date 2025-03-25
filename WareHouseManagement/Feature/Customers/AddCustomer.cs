using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using static System.Runtime.InteropServices.JavaScript.JSType;
using WareHouseManagement.Model.Entity;
using WareHouseManagement.Model.Entity.Customer_Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Customers {
    public class AddCustomer : IEndpoint {
        public record Request(string Name, string Address, string Email, string Phone, string? GroupId);
        public record Response(bool Success, string ErrorMessage, ValidationResult? ValidateError);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.Name).NotEmpty().WithMessage("Chưa nhập tên");
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Customers", Handler).WithTags("Customers");
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

                Customer Customer = new() {
                    Address = request.Address,
                    Name = request.Name,
                    Email = request.Email,
                    PhoneNumber = request.Phone,
                    CustomerGroup = await context.CustomerGroups.FindAsync(request.GroupId),
                    ServiceId = ServiceId,
                };
                await context.Customers.AddAsync(Customer);

                if (await context.SaveChangesAsync() > 0) {
                    return Results.Ok(new Response(true, "", ValidatedResult));
                }
                return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", ValidatedResult));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra!", null));
            }
           
        }

    }
}

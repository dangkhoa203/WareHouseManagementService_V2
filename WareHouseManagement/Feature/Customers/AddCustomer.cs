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

namespace WareHouseManagement.Feature.Customers
{
    public class AddCustomer:IEndpoint
    {
        public record Request(string name,string address,string email,string phone,string? groupId);
        public record Response(bool success, string errorMessage, ValidationResult? error);
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(r => r.name).NotEmpty().WithMessage("Chưa nhập tên");
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/Customers", Handler).WithTags("Customers");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Customer)]
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, ClaimsPrincipal user)
        {
            var service = context.Users.Include(u => u.ServiceRegistered).Where(u => u.UserName == user.Identity.Name).Select(u => u.ServiceRegistered).FirstOrDefault();
            var validator = new Validator();
            var validatedresult = validator.Validate(request);
            if (!validatedresult.IsValid)
            {
                return Results.BadRequest(new Response(false, "", validatedresult));
            }
            Customer customer = new()
            {
                Address = request.address,
                Name = request.name,
                Email = request.email,
                PhoneNumber = request.phone,
                CustomerGroup = await context.CustomerGroups.FindAsync(request.groupId),
                ServiceRegisteredFrom = service,
            };
            await context.Customers.AddAsync(customer);
            if (await context.SaveChangesAsync() > 0)
            {
                return Results.Ok(new Response(true, "", validatedresult));
            }
            return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", validatedresult));
        }

    }
}

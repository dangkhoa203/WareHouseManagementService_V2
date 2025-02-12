using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Vendor_EntiTy;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Vendors {
    public class AddVendor : IEndpoint {
        public record Request(string name, string address, string email, string phone, string? groupId);
        public record Response(bool success, string errorMessage, ValidationResult? error);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.name).NotEmpty().WithMessage("Chưa nhập tên");
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Vendors", Handler).WithTags("Vendors");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Vendor)]
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, ClaimsPrincipal user) {
            var service = context.Users.Include(u => u.ServiceRegistered).Where(u => u.UserName == user.Identity.Name).Select(u => u.ServiceRegistered).FirstOrDefault();
            var validator = new Validator();
            var validatedresult = validator.Validate(request);
            if (!validatedresult.IsValid) {
                return Results.BadRequest(new Response(false, "", validatedresult));
            }
            Vendor vendor = new() {
                Address = request.address,
                Name = request.name,
                Email = request.email,
                PhoneNumber = request.phone,
                VendorGroup = await context.VendorGroups.FindAsync(request.groupId),
                ServiceRegisteredFrom = service,
            };
            await context.Vendors.AddAsync(vendor);
            if (await context.SaveChangesAsync() > 0) {
                return Results.Ok(new Response(true, "", validatedresult));
            }
            return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", validatedresult));
        }
    }
}

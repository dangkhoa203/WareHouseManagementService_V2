using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Customer_Entity;
using WareHouseManagement.Model.Entity.Vendor_EntiTy;

namespace WareHouseManagement.Feature.Vendors
{
    public class UpdateVendor:IEndpoint
    {
        public record Request(string id, string name, string address, string email, string phone, string? groupId);
        public record Response(bool success, string errorMessage, ValidationResult? error);
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(r => r.name).NotEmpty().WithMessage("Chưa nhập tên");
            }
            private record checkmodel(string name, string address, string email, string phone, string? groupId);
            public bool checkSame(Request request, Vendor vendor)
            {
                checkmodel newDetail = new checkmodel(request.name, request.address, request.email, request.phone, request.groupId);
                checkmodel oldDetail = new checkmodel(vendor.Name, vendor.Address, vendor.Email, vendor.PhoneNumber, vendor.VendorGroup != null ? vendor.VendorGroup.Id : "");
                return oldDetail == newDetail;

            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/Vendors", Handler).RequireAuthorization().WithTags("Vendors");
        }
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, ClaimsPrincipal user)
        {

            var validator = new Validator();
            var validatedresult = validator.Validate(request);
            if (!validatedresult.IsValid)
            {
                return Results.BadRequest(new Response(false, "", validatedresult));
            }
            var service = context.Users.Include(u => u.ServiceRegistered).Where(u => u.UserName == user.Identity.Name).Select(u => u.ServiceRegistered).FirstOrDefault();
            var vendor = await context.Vendors
                .Include(c => c.VendorGroup)
                .Where(c => c.ServiceRegisteredFrom.Id == service.Id)
                .FirstOrDefaultAsync(c => c.Id == request.id);
            if (vendor == null)
                return Results.NotFound(new Response(false, "Lỗi xảy ra khi đang thực hiện!", validatedresult));

            if (!validator.checkSame(request, vendor))
            {
                vendor.Name = request.name;
                vendor.Email = request.email;
                vendor.PhoneNumber = request.phone;
                vendor.Address = request.address;
                vendor.VendorGroup = await context.VendorGroups.FindAsync(request.groupId);
                if (await context.SaveChangesAsync() < 1)
                {
                    return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", validatedresult));
                }
            }
            return Results.Ok(new Response(true, "", validatedresult));
        }
    }
}

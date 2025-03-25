using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Customer_Entity;
using WareHouseManagement.Model.Entity.Vendor_EntiTy;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Vendors {
    public class UpdateVendor : IEndpoint {
        public record Request(string Id, string Name, string Address, string Email, string Phone, string? GroupId);
        public record Response(bool Success, string ErrorMessage, ValidationResult? ValidateError);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.Name).NotEmpty().WithMessage("Chưa nhập tên");
            }
            private record Checkmodel(string Name, string Address, string Email, string Phone, string? GroupId);
            public bool checkSame(Request request, Vendor vendor) {
                Checkmodel NewDetail = new (request.Name, request.Address, request.Email, request.Phone, request.GroupId);
                Checkmodel OldDetail = new (vendor.Name, vendor.Address, vendor.Email, vendor.PhoneNumber, vendor.VendorGroup != null ? vendor.VendorGroup.Id : "");
                return OldDetail == NewDetail;

            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPut("/api/Vendors", Handler).WithTags("Vendors");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Vendor)]
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

                var Vendor = await context.Vendors
                    .Include(vendor => vendor.VendorGroup)
                    .Where(vendor => vendor.ServiceId == ServiceId)
                    .FirstOrDefaultAsync(vendor => vendor.Id == request.Id);

                if (Vendor == null)
                    return Results.NotFound(new Response(false, "Lỗi xảy ra khi đang thực hiện!", ValidatedResult));

                if (!Validator.checkSame(request, Vendor)) {
                    Vendor.Name = request.Name;
                    Vendor.Email = request.Email;
                    Vendor.PhoneNumber = request.Phone;
                    Vendor.Address = request.Address;
                    Vendor.VendorGroup = await context.VendorGroups.FindAsync(request.GroupId);
                    if (await context.SaveChangesAsync() < 1) {
                        return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", ValidatedResult));
                    }
                }

                return Results.Ok(new Response(true, "", ValidatedResult));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi server đã xảy ra khi đang thực hiện!", null));
            }
        }
    }
}

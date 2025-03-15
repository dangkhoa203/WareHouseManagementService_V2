using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Vendor_EntiTy;
using WareHouseManagement.Model.Entity.Warehouse_Entity;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.Warehouses {
    public class UpdateWarehouse:IEndpoint {
        public record Request(string Id, string Name, string Address, string City);
        public record Response(bool Success, string ErrorMessage, ValidationResult? ValidateError);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.Name).NotEmpty().WithMessage("Chưa nhập tên");
            }
            private record Checkmodel(string name, string address, string city);
            public bool checkSame(Request request, Warehouse Warehouse) {
                Checkmodel NewDetail = new (request.Name, request.Address, request.City);
                Checkmodel OldDetail = new (Warehouse.Name, Warehouse.Address, Warehouse.City);
                return OldDetail == NewDetail;

            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPut("/api/Warehouses", Handler).WithTags("Warehouses");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Warehouse)]
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

                var Warehouse = await context.Warehouses
                    .Where(warehouse => warehouse.ServiceId == ServiceId)
                    .FirstOrDefaultAsync(warehouse => warehouse.Id == request.Id);

                if (Warehouse == null)
                    return Results.NotFound(new Response(false, "Lỗi xảy ra khi đang thực hiện!", ValidatedResult));

                if (!Validator.checkSame(request, Warehouse)) {
                    Warehouse.Name = request.Name;
                    Warehouse.City = request.City;
                    Warehouse.Address = request.Address;
                    if (await context.SaveChangesAsync() < 1) {
                        return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", ValidatedResult));
                    }
                }

                return Results.Ok(new Response(true, "", ValidatedResult));
            }
            catch (Exception) {
                return Results.BadRequest(new Response(false, "Lỗi sever đã xảy ra!", null));
            }
        }
    }
}

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
        public record Request(string id, string name, string address, string city);
        public record Response(bool success, string errorMessage, ValidationResult? error);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.name).NotEmpty().WithMessage("Chưa nhập tên");
            }
            private record checkmodel(string name, string address, string city);
            public bool checkSame(Request request, Warehouse warehouse) {
                checkmodel newDetail = new checkmodel(request.name, request.address, request.city);
                checkmodel oldDetail = new checkmodel(warehouse.Name, warehouse.Address, warehouse.City);
                return oldDetail == newDetail;

            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPut("/api/Warehouses", Handler).WithTags("Warehouses");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Warehouse)]
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, ClaimsPrincipal user) {

            var validator = new Validator();
            var validatedresult = validator.Validate(request);
            if (!validatedresult.IsValid) {
                return Results.BadRequest(new Response(false, "", validatedresult));
            }
            var service = context.Users.Include(u => u.ServiceRegistered).Where(u => u.UserName == user.Identity.Name).Select(u => u.ServiceRegistered).FirstOrDefault();
            var warehouse = await context.Warehouses
                .Where(c => c.ServiceRegisteredFrom.Id == service.Id)
                .FirstOrDefaultAsync(c => c.Id == request.id);
            if (warehouse == null)
                return Results.NotFound(new Response(false, "Lỗi xảy ra khi đang thực hiện!", validatedresult));

            if (!validator.checkSame(request, warehouse)) {
                warehouse.Name = request.name;
                warehouse.City = request.city;
                warehouse.Address = request.address;
                if (await context.SaveChangesAsync() < 1) {
                    return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", validatedresult));
                }
            }
            return Results.Ok(new Response(true, "", validatedresult));
        }
    }
}

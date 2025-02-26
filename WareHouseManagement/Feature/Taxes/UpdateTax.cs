using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Product_Entity;
using WareHouseManagement.Model.Enum;
using WareHouseManagement.Model.Receipt;

namespace WareHouseManagement.Feature.Taxes {
    public class UpdateTax : IEndpoint {
        public record Request(string id, string name, string description, float percent);
        public record Response(bool success, string errorMessage, ValidationResult? error);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.name).NotEmpty().WithMessage("Chưa nhập tên");
                RuleFor(r => r.percent).LessThanOrEqualTo(100).GreaterThanOrEqualTo(0).WithMessage("Phần trăm chưa hợp lệ!");
            }
            private record checkModel(string name, string description, float percent);
            public bool checkSame(Request request, Tax Tax) {
                var oldDetail = new checkModel(Tax.Name, Tax.Description, Tax.Percent);
                var newDetail = new checkModel(request.name, request.description, request.percent);
                return oldDetail == newDetail;
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPut("/api/Taxes", Handler).WithTags("Taxes");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Tax)]
        private static async Task<IResult> Handler(
            Request request,
            ApplicationDbContext context,
            ClaimsPrincipal user) {
            var validator = new Validator();
            var validatedresult = validator.Validate(request);
            if (!validatedresult.IsValid)
                return Results.BadRequest(new Response(false, "", validatedresult));

            var serviceId = context.Users
                .Include(u => u.ServiceRegistered)
                .Where(u => u.UserName == user.Identity.Name)
                .Select(u => u.ServiceId)
                .FirstOrDefault();

            var tax = await context.Taxes
                .Where(u => u.ServiceId == serviceId)
                .FirstOrDefaultAsync(u => u.Id == request.id);
            if (tax == null)
                return Results.NotFound(new Response(false, "Lỗi xảy ra khi đang thực hiện!", validatedresult));

            if (!validator.checkSame(request, tax)) {
                tax.Name = request.name;
                tax.Description = request.description;
                if (await context.SaveChangesAsync() < 1) {
                    return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", validatedresult));
                }
            }
            return Results.Ok(new Response(true, "", validatedresult));
        }
    }
}

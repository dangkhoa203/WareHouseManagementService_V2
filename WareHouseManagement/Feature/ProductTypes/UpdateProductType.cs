using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Customer_Entity;
using WareHouseManagement.Model.Entity.Product_Entity;

namespace WareHouseManagement.Feature.ProductTypes
{
    public class UpdateProductType:IEndpoint
    {
        public record Request(string id, string name, string description);
        public record Response(bool success, string errorMessage, ValidationResult? error);
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(r => r.name).NotEmpty().WithMessage("Chưa nhập tên");
            }
            public bool checkSame(Request request, ProductType type)
            {
                return (request.name == type.Name && request.description == type.Description);
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/Product-Types", Handler).RequireAuthorization().WithTags("Product Types");
        }
        private static async Task<IResult> Handler(
            Request request,
            ApplicationDbContext context,
            ClaimsPrincipal user)
        {
            var validator = new Validator();
            var validatedresult = validator.Validate(request);
            if (!validatedresult.IsValid)
                return Results.BadRequest(new Response(false, "", validatedresult));

            var service = context.Users
                .Include(u => u.ServiceRegistered)
                .Where(u => u.UserName == user.Identity.Name)
                .Select(u => u.ServiceRegistered)
                .FirstOrDefault();

            var types = await context.ProductTypes
                .Where(t => t.ServiceRegisteredFrom.Id == service.Id)
                .FirstOrDefaultAsync(t => t.Id == request.id);
            if (types == null)
                return Results.NotFound(new Response(false, "Lỗi xảy ra khi đang thực hiện!", validatedresult));

            if (!validator.checkSame(request, types))
            {
                types.Name = request.name;
                types.Description = request.description;
                if (await context.SaveChangesAsync() < 1)
                {
                    return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", validatedresult));
                }
            }
            return Results.Ok(new Response(true, "", validatedresult));
        }
    }
}

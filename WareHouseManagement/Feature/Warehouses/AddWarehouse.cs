﻿using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Entity.Vendor_EntiTy;
using WareHouseManagement.Model.Entity.Warehouse_Entity;

namespace WareHouseManagement.Feature.Warehouses {
    public class AddWarehouse : IEndpoint {
        public record Request(string name, string address, string city);
        public record Response(bool success, string errorMessage, ValidationResult? error);
        public sealed class Validator : AbstractValidator<Request> {
            public Validator() {
                RuleFor(r => r.name).NotEmpty().WithMessage("Chưa nhập tên");
            }
        }
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapPost("/api/Warehouses", Handler).RequireAuthorization().WithTags("Warehouses");
        }
        private static async Task<IResult> Handler(Request request, ApplicationDbContext context, ClaimsPrincipal user) {
            var service = context.Users.Include(u => u.ServiceRegistered).Where(u => u.UserName == user.Identity.Name).Select(u => u.ServiceRegistered).FirstOrDefault();
            var validator = new Validator();
            var validatedresult = validator.Validate(request);
            if (!validatedresult.IsValid) {
                return Results.BadRequest(new Response(false, "", validatedresult));
            }
            var warehouse = new Warehouse() {
                Name = request.name,
                Address = request.address,
                City = request.city,
                ServiceRegisteredFrom = service
            };
            await context.Warehouses.AddAsync(warehouse);
            if (await context.SaveChangesAsync() > 0) {
                return Results.Ok(new Response(true, "", validatedresult));
            }
            return Results.BadRequest(new Response(false, "Lỗi xảy ra khi đang thực hiện!", validatedresult));
        }
    }
}

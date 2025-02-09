﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;

namespace WareHouseManagement.Feature.Warehouses {
    public class RemoveWarehouse:IEndpoint {
        public record Request(string id);
        public record Response(bool success, string errorMessage);
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapDelete("/api/Wareehouses/", Handler).RequireAuthorization().WithTags("Warehouses");
        }
        private static async Task<IResult> Handler([FromBody] Request request, ApplicationDbContext context, ClaimsPrincipal user) {
            var service = context.Users
                .Include(u => u.ServiceRegistered)
                .Where(u => u.UserName == user.Identity.Name)
                .Select(u => u.ServiceRegistered)
                .FirstOrDefault();
            var warehouse = await context.Warehouses
                .Where(t => t.ServiceRegisteredFrom.Id == service.Id)
                .FirstOrDefaultAsync(t => t.Id == request.id);
            if (warehouse != null) {
                context.Warehouses.Remove(warehouse);
                var result = await context.SaveChangesAsync();
                if (result > 0)
                    return Results.Ok(new Response(true, ""));
                return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!"));
            }
            return Results.NotFound(new Response(false, "Không tìm thấy nhóm!"));
        }
    }
}

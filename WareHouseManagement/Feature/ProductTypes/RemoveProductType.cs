﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.ProductTypes
{
    public class RemoveProductType:IEndpoint
    {
        public record Request(string id);
        public record Response(bool success, string errorMessage);
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/Product-Types/", Handler).WithTags("Product Types");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Product)]
        private static async Task<IResult> Handler([FromBody] Request request, ApplicationDbContext context, ClaimsPrincipal user)
        {
            var service = context.Users
                .Include(u => u.ServiceRegistered)
                .Where(u => u.UserName == user.Identity.Name)
                .Select(u => u.ServiceRegistered)
                .FirstOrDefault();
            var type = await context.ProductTypes
                .Where(u => u.ServiceRegisteredFrom.Id == service.Id)
                .FirstOrDefaultAsync(u => u.Id == request.id);
            if (type != null)
            {
                context.ProductTypes.Remove(type);
                var result = await context.SaveChangesAsync();
                if (result > 0)
                    return Results.Ok(new Response(true, ""));
                return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!"));
            }
            return Results.NotFound(new Response(false, "Không tìm thấy nhóm!"));
        }
    }
}

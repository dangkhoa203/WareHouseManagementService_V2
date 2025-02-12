﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.VendorGroups
{
    public class RemoveVendorGroup:IEndpoint
    {
        public record Request(string id);
        public record Response(bool success, string errorMessage);
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/Vendor-Groups/", Handler).WithTags("Vendor Groups");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Vendor)]
        private static async Task<IResult> Handler([FromBody] Request request, ApplicationDbContext context, ClaimsPrincipal user)
        {
            var service = context.Users
                .Include(u => u.ServiceRegistered)
                .Where(u => u.UserName == user.Identity.Name)
                .Select(u => u.ServiceRegistered)
                .FirstOrDefault();
            var group = await context.VendorGroups
                .Where(u => u.ServiceRegisteredFrom.Id == service.Id)
                .FirstOrDefaultAsync(u => u.Id == request.id);
            if (group != null)
            {
                context.VendorGroups.Remove(group);
                var result = await context.SaveChangesAsync();
                if (result > 0)
                    return Results.Ok(new Response(true, ""));
                return Results.BadRequest(new Response(false, "Lỗi đã xảy ra!"));
            }
            return Results.NotFound(new Response(false, "Không tìm thấy nhóm!"));
        }
    }
}

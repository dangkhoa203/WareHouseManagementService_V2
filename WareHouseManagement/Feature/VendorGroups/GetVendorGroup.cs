﻿using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;

namespace WareHouseManagement.Feature.VendorGroups
{
    public class GetVendorGroup:IEndpoint
    {
        public record groupDTO(string id, string name, string description, DateTime createDate);
        public record Response(bool success, groupDTO data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/Vendor-Groups/{id}", Handler).WithTags("VendorGroups");
        }
        private static async Task<IResult> Handler(string id,ApplicationDbContext context, ClaimsPrincipal user)
        {
            try
            {
                var service = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceRegistered)
                    .FirstOrDefault();
                var group = await context.VendorGroups
                    .Where(g => g.ServiceRegisteredFrom.Id == service.Id)
                    .OrderByDescending(g => g.CreatedDate)
                    .Where(g => g.Id == id)
                    .Select(g => new groupDTO(g.Id, g.Name, g.Description, g.CreatedDate))
                    .FirstOrDefaultAsync();
                if (group != null)
                    return Results.Ok(new Response(true, group, ""));
                return Results.NotFound(new Response(false, null, "Không tìm thấy dữ liệu!"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new Response(false, null, "Lỗi đã xảy ra!"));
            }
        }
    }
}

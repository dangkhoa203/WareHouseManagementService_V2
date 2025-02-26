﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.CustomerGroups
{
    public class GetCustomerGroup:IEndpoint
    {
        public record groupDTO(string id, string name, string description, DateTime createDate);
        public record Response(bool success, groupDTO? data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/Customer-Groups/{id}", Handler).WithTags("Customer Groups");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Customer)]
        private static async Task<IResult> Handler([FromRoute]string id,ApplicationDbContext context, ClaimsPrincipal user)
        {
            try
            {
                var serviceId = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceId)
                    .FirstOrDefault();
                var group = await context.CustomerGroups
                    .Where(u => u.ServiceId == serviceId)
                    .OrderByDescending(u => u.CreatedDate)
                    .Where(u=>u.Id==id)
                    .Select(u => new groupDTO(u.Id, u.Name, u.Description, u.CreatedDate))
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

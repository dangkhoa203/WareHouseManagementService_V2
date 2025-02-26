﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WareHouseManagement.Data;
using WareHouseManagement.Endpoint;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Feature.ExportForms {
    public class GetExportForms:IEndpoint {
        public record receiptDTO(string id, string customerName, DateTime dateOfOrder);
        public record formDTO(string id, receiptDTO receipt, DateTime dateOfExport);
        public record Response(bool success, List<formDTO> data, string errorMessage);

        public static void MapEndpoint(IEndpointRouteBuilder app) {
            app.MapGet("/api/Export-Forms", Handler).WithTags("Import Forms");
        }
        [Authorize(Roles = Permission.Admin + "," + Permission.Stock)]
        private static async Task<IResult> Handler(ApplicationDbContext context, ClaimsPrincipal user) {
            try {
                var serviceId = context.Users
                    .Include(u => u.ServiceRegistered)
                    .Where(u => u.UserName == user.Identity.Name)
                    .Select(u => u.ServiceId)
                    .FirstOrDefault();
                var forms = await context.StockExportForms
                    .Include(f => f.Receipt)
                    .ThenInclude(re => re.Customer)
                    .Where(f => f.ServiceId == serviceId)
                    .OrderByDescending(f => f.CreatedDate)
                    .Select(f => new formDTO(
                        f.Id,
                        new receiptDTO(
                            f.ReceiptId,
                            f.Receipt.Customer.Name,
                            f.Receipt.DateOrder
                            ),
                        f.ExportDate
                    ))
                    .ToListAsync();
                return Results.Ok(new Response(true, forms, ""));
            } catch (Exception ex) {
                return Results.BadRequest(new Response(false, [], "Lỗi đã xảy ra!"));
            }
        }
    }
}

using Microsoft.AspNetCore.Identity;
using WareHouseManagement.Model.Entity;
using WareHouseManagement.Model.Enum;

namespace WareHouseManagement.Data {
    public class DbInitializer : IDbInitializer {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Account> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbInitializer(ApplicationDbContext context, UserManager<Account> userManager, RoleManager<IdentityRole> roleManager) {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async void Initialize() {
            _context.Database.EnsureCreated();
            if (!await _roleManager.RoleExistsAsync(Permission.Admin)) {
                await _roleManager.CreateAsync(new IdentityRole(Permission.Admin));
            }
            if (!await _roleManager.RoleExistsAsync(Permission.Product)) {
                await _roleManager.CreateAsync(new IdentityRole(Permission.Product));
            }
            if (!await _roleManager.RoleExistsAsync(Permission.Vendor)) {
                await _roleManager.CreateAsync(new IdentityRole(Permission.Vendor));
            }
            if (!await _roleManager.RoleExistsAsync(Permission.Customer)) {
                await _roleManager.CreateAsync(new IdentityRole(Permission.Customer));
            }
            if (!await _roleManager.RoleExistsAsync(Permission.VendorReceipt)) {
                await _roleManager.CreateAsync(new IdentityRole(Permission.VendorReceipt));
            }
            if (!await _roleManager.RoleExistsAsync(Permission.CustomerReceipt)) {
                await _roleManager.CreateAsync(new IdentityRole(Permission.CustomerReceipt));
            }
            if (!await _roleManager.RoleExistsAsync(Permission.Stock)) {
                await _roleManager.CreateAsync(new IdentityRole(Permission.Stock));
            }
            if (!await _roleManager.RoleExistsAsync(Permission.Import)) {
                await _roleManager.CreateAsync(new IdentityRole(Permission.Import));
            }
            if (!await _roleManager.RoleExistsAsync(Permission.Export)) {
                await _roleManager.CreateAsync(new IdentityRole(Permission.Export));
            }

        }
    }
}

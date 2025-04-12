using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Middleware.Config;
using WareHouseManagement.Data;
using WareHouseManagement.Extensions;
using WareHouseManagement.Middleware;
using WareHouseManagement.Model.Entity.Account;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.WithOrigins("http://localhost:7088")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials()
         ;
    }); ;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen(
    options => {
        options.CustomSchemaIds(s => s.FullName?.Replace("+", "."));
    }
);
builder.Services.Configure<EmailSenderConfig>(builder.Configuration.GetSection("EmailSenderConfig"));
builder.Services.AddOptions();
builder.Services.AddScoped<EmailSender>();
builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddIdentityApiEndpoints<Account>().AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentityCore<Account>(option => {

    option.SignIn.RequireConfirmedAccount = true;
    option.Password.RequireUppercase = false;
    option.Password.RequireLowercase = false;
    option.Password.RequireDigit = false;
    option.Password.RequireNonAlphanumeric = false;
    option.Password.RequiredLength = 3;
    option.Password.RequiredUniqueChars = 0;
    option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    option.Lockout.MaxFailedAccessAttempts = 5;
    option.Lockout.AllowedForNewUsers = true;
    option.User.RequireUniqueEmail = false;
}).AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.ConfigureApplicationCookie(options => {
    options.Cookie.SameSite = SameSiteMode.None;
});
var app = builder.Build();
app.UseCors();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.AddAllEndPoint();
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapIdentityApi<Account>();

app.Run();
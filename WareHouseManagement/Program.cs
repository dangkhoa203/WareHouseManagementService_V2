using Microsoft.AspNetCore.Identity;
using WareHouseManagement.Data;
using WareHouseManagement.Extensions;
using WareHouseManagement.Model.Entity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options => {
    options.AddPolicy(name: "Dev", policy => {
        policy.WithOrigins("http://localhost:7088")
        .WithOrigins("http://localhost:7089")
        .WithOrigins("http://localhost:7090")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        ;
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.CustomSchemaIds(s => s.FullName?.Replace("+", "."));
    }
    );
builder.Services.AddDbContext<ApplicationDbContext>();
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
    option.User.RequireUniqueEmail = true;
}).AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.ConfigureApplicationCookie(options => {
    options.Cookie.SameSite = SameSiteMode.None;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.AddAllEndPoint();
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapIdentityApi<Account>();
app.UseCors("Dev");
app.MapControllers();
app.Run();

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using XWingTO.Core;
using XWingTO.Data;
using DbContext = XWingTO.Data.DbContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<XWingTO.Data.DbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("XWingTO")));
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

builder.Services.AddControllersWithViews();

builder.Services.AddIdentityCore<ApplicationUser>()
	.AddEntityFrameworkStores<DbContext>();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    options.User.RequireUniqueEmail = true;
});

var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseStatusCodePages();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();

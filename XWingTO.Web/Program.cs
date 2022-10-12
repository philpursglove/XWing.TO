using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using XWingTO.Core;
using XWingTO.Data;
using DbContext = XWingTO.Data.DbContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("XWingTO")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddEntityFrameworkStores<DbContext>();
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

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
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
	endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
	endpoints.MapRazorPages();
});

app.Run();

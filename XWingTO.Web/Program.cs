using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using XWingTO.Core;
using XWingTO.Data;
using DbContext = XWingTO.Data.DbContext;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("XWingTO")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddEntityFrameworkStores<DbContext>();
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

List<string> quotes = File.ReadAllLines("quotes.txt").ToList();

builder.Services.AddSingleton(typeof(List<string>), quotes);

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
builder.Services.AddAzureClients(clientBuilder =>
{
	clientBuilder.AddQueueServiceClient(builder.Configuration["XWingTO.Queue"], preferMsi: true);
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

if (builder.Configuration.GetValue<bool>("MigrateDatabase"))
{
	using (var scope = app.Services.CreateScope())
	{
		var db = scope.ServiceProvider.GetRequiredService<DbContext>();
		await db.Database.MigrateAsync();
	}
}

app.Run();

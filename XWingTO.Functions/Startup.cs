using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XWingTO.Data;
using DbContext = XWingTO.Data.DbContext;

[assembly: FunctionsStartup(typeof(XWingTO.Functions.Startup))]

namespace XWingTO.Functions
{
	public class Startup : FunctionsStartup
	{
		private readonly IConfiguration _configuration;
		public Startup(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public override void Configure(IFunctionsHostBuilder builder)
		{
			builder.Services.AddOptions<Options>()
				.Configure<IConfiguration>((settings, configuration) =>
				{
					configuration.GetSection("MyOptions").Bind(settings);
				});

			builder.Services.AddDbContext<DbContext>(options => options.UseSqlServer());
			builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
		}
	}
}
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using XWingTO.Data;
using DbContext = XWingTO.Data.DbContext;

[assembly: FunctionsStartup(typeof(XWingTO.Functions.Startup))]

namespace XWingTO.Functions
{
	public class Startup : FunctionsStartup
	{
		public override void Configure(IFunctionsHostBuilder builder)
		{
			builder.Services.AddOptions<Options>()
				.Configure<IConfiguration>((settings, configuration) =>
				{
					configuration.GetSection("MyOptions").Bind(settings);
				});

			var configuration = builder.Services.BuildServiceProvider().GetService<IConfiguration>();

			builder.Services.AddDbContext<DbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DbConnectionString")));
			builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
		}

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();

            builder.ConfigurationBuilder
                .SetBasePath(context.ApplicationRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.{context.EnvironmentName}.json", optional: true, reloadOnChange: false)
                .AddUserSecrets(Assembly.GetExecutingAssembly(), true, true)
                .AddEnvironmentVariables();
        }
    }
}
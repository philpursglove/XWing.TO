using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XWingTO.Data;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        var configuration = services.BuildServiceProvider().GetService<IConfiguration>();

        services.AddDbContext<DbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DbConnectionString")));
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
    })
    .Build();

host.Run();
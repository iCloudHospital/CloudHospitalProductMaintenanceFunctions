using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CloudHospital.ProductMaintenanceFunctions;

namespace CloudHospital_ProductMaintenanceFunctions
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(ConfigureService)
                .Build();

            host.Run();
        }

        public static void ConfigureService(HostBuilderContext context, IServiceCollection services)
        {
            services.AddOptions<BlobStorageOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                configuration.GetSection("BlobStorageOptions").Bind(options);
            });

            services.AddOptions<AuthorizationOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                configuration.GetSection("AuthorizationOptions").Bind(options);
            });
        }
    }
}
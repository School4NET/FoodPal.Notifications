using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using MassTransit;
using FoodPal.Notifications.Processor.Consumers;
using FoodPal.Notifications.Data;
using FoodPal.Notifications.Data.Abstractions;
using FoodPal.Notifications.Common.Settings;

namespace FoodPal.Notifications.Processor
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureServices)
                .ConfigureAppConfiguration(ConfigureAppConfiguration)
                .RunConsoleAsync();
        }

        private static void ConfigureAppConfiguration(HostBuilderContext hostBuilder, IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.SetBasePath(hostBuilder.HostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json");
        }

        private static void ConfigureServices(HostBuilderContext hostBuilder, IServiceCollection services)
        {
            services.AddHostedService<MassTransitConsoleHostedService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.Configure<DbSettings>(hostBuilder.Configuration.GetSection("ConnectionStrings"));
            services.AddScoped<NotificationDbContext>();

            services.AddScoped<NewUserAddedConsumer>();

            services.AddMassTransit(configuration => {
                configuration.UsingAzureServiceBus((context, config) =>
                {
                    config.Host("Endpoint=sb://school4net.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=uAKNEmB5x9Me8/Fl2t6Os6I9EJcRANHUE0jfnxk3phU=");

                    config.ReceiveEndpoint("notifications-users-queue", e =>
                    {
                        // register consumer
                        e.Consumer(() => context.GetService<NewUserAddedConsumer>());
                    });
                });
            });
        }
    }
}

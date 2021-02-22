using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using MassTransit;
using FoodPal.Notifications.Processor.Consumers;

namespace FoodPal.Notifications.Processor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureServices)
                .ConfigureAppConfiguration(ConfigureAppConfiguration)
                .RunConsoleAsync();
        }

        private static void ConfigureAppConfiguration(HostBuilderContext arg1, IConfigurationBuilder arg2)
        { 
        }

        private static void ConfigureServices(HostBuilderContext hostBuilder, IServiceCollection services)
        {
            services.AddHostedService<MassTransitConsoleHostedService>();

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

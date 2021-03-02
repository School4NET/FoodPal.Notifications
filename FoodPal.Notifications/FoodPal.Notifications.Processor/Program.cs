using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using MassTransit;
using FoodPal.Notifications.Data;
using FoodPal.Notifications.Data.Abstractions;
using FoodPal.Notifications.Common.Settings;
using FoodPal.Notifications.Mappers;
using FluentValidation;
using FoodPal.Notifications.Validations;
using FoodPal.Notifications.Processor.Messages.Consumers;
using MediatR;
using FoodPal.Notifications.Application.Handlers;
using FoodPal.Notifications.Messages;
using FoodPal.Notifications.Service;
using FoodPal.Notifications.Service;
using FoodPal.Notifications.Service.Email;
using FoodPal.Notifications.Service.Email;
using FoodPal.Notifications.Processor.Workers;

namespace FoodPal.Notifications.Processor
{
    class Program
    {
        static IConfiguration Configuration;

        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(ConfigureAppConfiguration)
                .ConfigureServices(ConfigureServices)
                .RunConsoleAsync();
        }

        private static void ConfigureAppConfiguration(HostBuilderContext hostBuilder, IConfigurationBuilder configurationBuilder)
        { 
            configurationBuilder.SetBasePath(hostBuilder.HostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json")
                .AddUserSecrets<Program>();

            Configuration = configurationBuilder.Build();
        }

        private static void ConfigureServices(HostBuilderContext hostBuilder, IServiceCollection services)
        {
            var messageBrokerSettings = Configuration.GetSection("MessageBroker").Get<MessageBrokerSettings>(); 
            services.Configure<NotificationServiceSettings>(hostBuilder.Configuration.GetSection("NotificationServiceSettings"));

            services.AddHostedService<MassTransitConsoleHostedService>();
            services.AddHostedService<NotificationRetryHostedService>();

            services.AddValidatorsFromAssembly(typeof(InternalValidator<>).Assembly);

            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IEmailNotificationService, EmailNotificationService>();

            services.AddAutoMapper(typeof(InternalProfile).Assembly);
            services.AddMediatR(typeof(NewUserAddedCommandHandler).Assembly); 

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.Configure<DbSettings>(hostBuilder.Configuration.GetSection("ConnectionStrings"));
            services.AddScoped<NotificationDbContext>();

            services.AddScoped<NewUserAddedConsumer>();
            services.AddScoped<NewNotificationAddedConsumer>();
            services.AddScoped<UserUpdatedConsumer>();
            services.AddScoped<NotificationViewedConsumer>();
            services.AddScoped<UserNotificationsRequestedConsumer>();
            services.AddScoped<DeliveryAddedConsumer>();
            services.AddScoped<DeliveryCompletedConsumer>();

            services.AddMassTransit(configuration => {
                configuration.UsingAzureServiceBus((context, config) =>
                {
                    config.Host(messageBrokerSettings.ServiceBusHost); 

                    config.ReceiveEndpoint("notifications-users-queue", e =>
                    {
                        // register consumer 
                        e.Consumer(() => context.GetService<NewUserAddedConsumer>()); 
                        e.Consumer(() => context.GetService<UserUpdatedConsumer>());
                    });

                    config.ReceiveEndpoint("notifications-queue", e =>
                    {
                        // register consumer 
                        e.Consumer(() => context.GetService<NewNotificationAddedConsumer>());
                        e.Consumer(() => context.GetService<NotificationViewedConsumer>());
                        e.Consumer(() => context.GetService<UserNotificationsRequestedConsumer>());
                    });

                    config.ReceiveEndpoint("notifications-delivery-queue", e =>
                    {
                        // register consumer 
                        e.Consumer(() => context.GetService<DeliveryCompletedConsumer>());
                        e.Consumer(() => context.GetService<DeliveryAddedConsumer>());
                    });
                });
            });
        }
    }
}

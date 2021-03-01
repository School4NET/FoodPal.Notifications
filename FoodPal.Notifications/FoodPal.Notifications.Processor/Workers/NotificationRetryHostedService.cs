using FoodPal.Notifications.Data.Abstractions;
using FoodPal.Notifications.Domain;
using FoodPal.Notifications.Dto.Intern;
using FoodPal.Notifications.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FoodPal.Notifications.Processor.Workers
{
    public class NotificationRetryHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<NotificationRetryHostedService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private Timer _timer;

        public NotificationRetryHostedService(ILogger<NotificationRetryHostedService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            this._logger = logger;
            this._serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            this._logger.LogInformation("NotificationRetryHostedService service started.");

            this._timer = new Timer(SendNotifications, null, TimeSpan.Zero, TimeSpan.FromMinutes(5)); // change to 5 or move it to configuration

            return Task.CompletedTask;
        }

        private async void SendNotifications(object state)
        {
            using (var scope = this._serviceScopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                var notificationService = scope.ServiceProvider.GetService<INotificationService>();

                var errorNotifications = unitOfWork.GetRepository<Domain.Notification>().Find(x => x.Status == Common.Enums.NotificationStatusEnum.Error, new List<string>() { nameof(User) });
                foreach (var notification in errorNotifications)
                {
                    var notificationServiceDto = new NotificationServiceDto
                    {
                        Body = notification.Message,
                        Email = notification.User.Email,
                        Subject = notification.Title,
                        PhoneNo = notification.User.PhoneNo
                    };
                    var sent = await notificationService.Send(notification.Type, notificationServiceDto);

                    // change the notification status
                    notification.Status = sent ? Common.Enums.NotificationStatusEnum.Viewed : Common.Enums.NotificationStatusEnum.Error;
                }
                await unitOfWork.SaveChangesAsnyc();
            } 
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NotificationRetryHostedService service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

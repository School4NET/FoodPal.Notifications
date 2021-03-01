using FoodPal.Contracts;
using FoodPal.Notifications.Application.Commands;
using FoodPal.Notifications.Application.Queries;
using FoodPal.Notifications.Domain;
using FoodPal.Notifications.Dto;
using FoodPal.Notifications.Processor.Commands;

namespace FoodPal.Notifications.Mappers
{
    public class NotificationMapper : InternalProfile
    {
        public NotificationMapper()
        {
            this.CreateMap<INewNotificationAddedEvent, NewNotificationAddedCommand>();
            this.CreateMap<NewNotificationAddedCommand, Notification>();

            this.CreateMap<INotificationViewedEvent, NotificationViewedCommand>();
            this.CreateMap<Notification, NotificationDto>();
            this.CreateMap<IUserNotificationsRequested, UserNotificationRequestedQuery>();
        }
    }
}

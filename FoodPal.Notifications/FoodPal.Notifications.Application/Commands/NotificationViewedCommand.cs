using MediatR;

namespace FoodPal.Notifications.Application.Commands
{
    public class NotificationViewedCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}

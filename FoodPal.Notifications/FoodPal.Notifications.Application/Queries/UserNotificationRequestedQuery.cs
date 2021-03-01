using FoodPal.Notifications.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodPal.Notifications.Application.Queries
{
    public class UserNotificationRequestedQuery : IRequest<NotificationsDto>
    {
        public int UserId { get; set; }
    }
}

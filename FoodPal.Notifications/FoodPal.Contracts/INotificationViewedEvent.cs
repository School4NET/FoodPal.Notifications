using FoodPal.Notifications.Common.Enums;
using System;
namespace FoodPal.Contracts
{
    public interface INotificationViewedEvent
    {
        public int Id { get; set; } 
    }
}

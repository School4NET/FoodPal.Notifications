using System;
using System.Collections.Generic;
using System.Text;

namespace FoodPal.Contracts
{
    public interface IDeliveryCompletedEvent
    {
        string Title { get; set; }
        string Message { get; set; }
        int UserId { get; set; }
        string CreateBy { get; set; }
        string ModifiedBy { get; set; }
        NotificationTypeEnum Type { get; set; } 
        string Info { get; set; }
         
        public enum NotificationTypeEnum
        {
            InApp,
            Text,
            Email
        }
    } 
}

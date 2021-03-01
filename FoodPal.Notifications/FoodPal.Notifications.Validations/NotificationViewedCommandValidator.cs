using FluentValidation;
using FoodPal.Notifications.Application.Commands;
using FoodPal.Notifications.Processor.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodPal.Notifications.Validations
{
    public class NotificationViewedCommandValidator : InternalValidator<NotificationViewedCommand>
    {
        public NotificationViewedCommandValidator()
        {
            this.RuleFor(x => x.Id).NotEmpty(); 
        }
    }
}

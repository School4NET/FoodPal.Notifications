using FluentValidation;
using FoodPal.Notifications.Application.Commands;
using FoodPal.Notifications.Application.Queries;
using FoodPal.Notifications.Processor.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodPal.Notifications.Validations
{
    public class UserNotificationRequestedQueryValidator : InternalValidator<UserNotificationRequestedQuery>
    {
        public UserNotificationRequestedQueryValidator()
        {
            this.RuleFor(x => x.UserId).NotEmpty(); 
        }
    }
}

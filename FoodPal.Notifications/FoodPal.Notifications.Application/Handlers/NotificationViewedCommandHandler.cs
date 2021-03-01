using AutoMapper;
using FluentValidation;
using FoodPal.Notifications.Service;
using FoodPal.Notifications.Application.Extensions;
using FoodPal.Notifications.Data.Abstractions;
using FoodPal.Notifications.Domain; 
using FoodPal.Notifications.Dto.Intern;
using FoodPal.Notifications.Processor.Commands;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FoodPal.Notifications.Application.Commands;
using System;

namespace FoodPal.Notifications.Application.Handlers
{
    public class NotificationViewedCommandHandler : IRequestHandler<NotificationViewedCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork; 
        private readonly IValidator<NotificationViewedCommand> _validator; 

        public NotificationViewedCommandHandler(IUnitOfWork unitOfWork, IValidator<NotificationViewedCommand> validator)
        {
            this._unitOfWork = unitOfWork; 
            this._validator = validator; 
        }

        public async Task<bool> Handle(NotificationViewedCommand request, CancellationToken cancellationToken)
        {
            this._validator.ValidateAndThrowEx(request);

            // save to db
            var notificationModel = await this._unitOfWork.GetRepository<Domain.Notification>().FindByIdAsync(request.Id);
            notificationModel.Status = Common.Enums.NotificationStatusEnum.Viewed;
            notificationModel.ModifiedAt = DateTimeOffset.Now;
            notificationModel.ModifiedBy = "system";
            var saved = await this._unitOfWork.SaveChangesAsnyc(); 

            return saved;
        }
    }
}

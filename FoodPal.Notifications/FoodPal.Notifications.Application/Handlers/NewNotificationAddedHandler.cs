﻿using AutoMapper;
using FluentValidation;
using FoodPal.Notification.Service;
using FoodPal.Notifications.Common.Extensions;
using FoodPal.Notifications.Data.Abstractions;
using FoodPal.Notifications.Domain;
using FoodPal.Notifications.Dto.Exceptions;
using FoodPal.Notifications.Dto.Intern;
using FoodPal.Notifications.Processor.Commands;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FoodPal.Notifications.Application.Handlers
{
    public class NewNotificationAddedHandler : IRequestHandler<NewNotificationAddedCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<NewNotificationAddedCommand> _validator;
        private readonly INotificationService _notificationService;

        public NewNotificationAddedHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<NewNotificationAddedCommand> validator, INotificationService notificationService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._validator = validator;
            this._notificationService = notificationService; 
        }

        public async Task<bool> Handle(NewNotificationAddedCommand request, CancellationToken cancellationToken)
        {
            var notificationModel = this._mapper.Map<Domain.Notification>(request);

            this._validator.ValidateAndThrowEx(request);

            // save to db
            this._unitOfWork.GetRepository<Domain.Notification>().Create(notificationModel);
            var saved = await this._unitOfWork.SaveChangesAsnyc();

            // TODO: refactor this
            var userModel = await this._unitOfWork.GetRepository<User>().FindByIdAsync(notificationModel.UserId);
            var notificationServiceDto = new NotificationServiceDto
            {
                Body = notificationModel.Message,
                Email = userModel.Email,
                Subject = notificationModel.Title,
                PhoneNo = userModel.PhoneNo
            };
            var sent = await this._notificationService.Send(notificationModel.Type, notificationServiceDto);

            if (sent)
            {
                notificationModel.Status = Common.Enums.NotificationStatusEnum.Viewed;
                await this._unitOfWork.SaveChangesAsnyc();
            }

            return saved && sent;
        }
    }
}
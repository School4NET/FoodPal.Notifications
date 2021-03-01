using AutoMapper;
using FluentValidation;
using FoodPal.Notifications.Application.Extensions;
using FoodPal.Notifications.Application.Queries;
using FoodPal.Notifications.Data.Abstractions;
using FoodPal.Notifications.Domain;
using FoodPal.Notifications.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FoodPal.Notifications.Application.Handlers
{
    public class UserNotificationsRequestedQueryHandler : IRequestHandler<UserNotificationRequestedQuery, NotificationsDto>
    {
        private readonly IUnitOfWork _unitOfWork; 
        private readonly IValidator<UserNotificationRequestedQuery> _validator;
        private readonly IMapper _mapper;

        public UserNotificationsRequestedQueryHandler(IUnitOfWork unitOfWork, IValidator<UserNotificationRequestedQuery> validator, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._validator = validator; 
        }

        public async Task<NotificationsDto> Handle(UserNotificationRequestedQuery request, CancellationToken cancellationToken)
        {
            this._validator.ValidateAndThrowEx(request);

            var notifications = this._unitOfWork.GetRepository<Notification>().Find(x => x.UserId == request.UserId && 
                                                                                    x.Status == Common.Enums.NotificationStatusEnum.Created &&
                                                                                    x.Type == Common.Enums.NotificationTypeEnum.InApp
                                                                                ).ToList();
            var notificationsDtos = this._mapper.Map<List<NotificationDto>>(notifications);

            return new NotificationsDto
            {
                Notifications = notificationsDtos
            };
        }
    }
}

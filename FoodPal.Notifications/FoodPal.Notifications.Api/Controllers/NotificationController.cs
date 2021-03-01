using FoodPal.Contracts;
using FoodPal.Notifications.Api.Exceptions;
using FoodPal.Notifications.Dto;
using FoodPal.Notifications.Dto.Intern;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FoodPal.Notifications.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {  
        private readonly ILogger<NotificationController> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRequestClient<IUserNotificationsRequested> _requestClientUserNotificationsRequested;

        public NotificationController(ILogger<NotificationController> logger, IPublishEndpoint publishEndpoint, IRequestClient<IUserNotificationsRequested> requestClientUserNotificationsRequested)
        {
            this._logger = logger;
            this._publishEndpoint = publishEndpoint;
            this._requestClientUserNotificationsRequested = requestClientUserNotificationsRequested;
        } 

        [HttpPost]
        public async Task<IActionResult> CreateNotification(NotificationDto notificationDto)
        { 
            await this._publishEndpoint.Publish<INewNotificationAddedEvent>(notificationDto);

            return Accepted();
        }

        [Route("viewed/{id}")]
        [HttpPatch]
        public async Task<IActionResult> ViewNotification(int id)
        {
            await this._publishEndpoint.Publish<INotificationViewedEvent>(new { 
                Id = id
            });

            return Accepted();
        }
         
        [Route("userId")]
        [HttpGet]
        public async Task<IActionResult> GetUserNotifications(int userId)
        {
            var response = await this._requestClientUserNotificationsRequested.GetResponse<NotificationsDto, InternalErrorResponseDto>(new
            {
                UserId = userId
            });

            if (response.Is<InternalErrorResponseDto>(out var respError))
            {
                throw new HttpResponseException(respError.Message);
            }

            return Ok(response.Message);
        }
    }
}

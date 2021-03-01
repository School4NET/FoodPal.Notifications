using FoodPal.Notifications.Dto.Intern;
using System;

namespace FoodPal.Notifications.Api.Exceptions
{
    public class HttpResponseException : Exception
    {
        public InternalErrorResponseDto ExceptionDto { get; private set; }

        public HttpResponseException(InternalErrorResponseDto internalResponseDto)
        {
            this.ExceptionDto = internalResponseDto;   
        }
    }
}

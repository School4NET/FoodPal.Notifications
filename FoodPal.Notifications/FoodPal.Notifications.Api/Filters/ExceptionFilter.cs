using FoodPal.Notifications.Api.Dto;
using FoodPal.Notifications.Api.Exceptions;
using FoodPal.Notifications.Dto.Intern;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FoodPal.Notifications.Api.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            this._logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            this._logger.LogError(context.Exception, "Something went wrong");

            HttpErrorResponseDto httpErrorResponseDto = null;

            if (context.Exception is HttpResponseException httpResponseException)
            {
                if (httpResponseException.ExceptionDto.Type == InternalErrorResponseTypeEnum.Validation)
                {
                    httpErrorResponseDto = new HttpErrorResponseDto
                    {
                        Details = httpResponseException.ExceptionDto.Details,
                        Error = httpResponseException.ExceptionDto.Message,
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }
            }  
            else 
            {
                httpErrorResponseDto = new HttpErrorResponseDto
                {
                    Details = "Please contact the admin",
                    Error = "Something went wrong",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }

            context.Result = new JsonResult(httpErrorResponseDto);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodPal.Notifications.Dto.Intern
{
    public class InternalErrorResponseDto
    {
        public string Message { get; set; }
        public string Details { get; set; }
        public InternalErrorResponseTypeEnum Type { get; set; }
    }

    public enum InternalErrorResponseTypeEnum
    {
        Error,
        Validation
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodPal.Contracts
{
    public interface IUserNotificationsRequested
    {
        public int UserId { get; set; }
    }
}

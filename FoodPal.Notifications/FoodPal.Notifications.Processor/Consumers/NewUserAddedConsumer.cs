using FoodPal.Contracts;
using MassTransit;
using System.Threading.Tasks;

namespace FoodPal.Notifications.Processor.Consumers
{
    public class NewUserAddedConsumer : IConsumer<INewUserAdded>
    {
        public async Task Consume(ConsumeContext<INewUserAdded> context)
        {
           var message = context.Message;
        }
    }
}

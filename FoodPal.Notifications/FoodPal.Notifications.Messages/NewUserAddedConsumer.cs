using AutoMapper;
using FoodPal.Contracts;
using FoodPal.Notifications.Common.Exceptions;
using FoodPal.Notifications.Processor.Commands;
using MassTransit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FoodPal.Notifications.Processor.Messages.Consumers
{
    public class NewUserAddedConsumer : IConsumer<INewUserAddedEvent>
    {  
        private readonly IMapper _mapper;
        private readonly ILogger<NewUserAddedConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public NewUserAddedConsumer(IMediator mediator, IMapper mapper, ILogger<NewUserAddedConsumer> logger, IServiceScopeFactory serviceScopeFactory)
        {  
            this._mapper = mapper;
            this._logger = logger;
            this._serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Consume(ConsumeContext<INewUserAddedEvent> context)
        {
            try
            {
                var message = context.Message;

                var command = this._mapper.Map<NewUserAddedCommand>(message); 

                using (var scope = this._serviceScopeFactory.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    await mediator.Send(command);
                }
            }
            catch (ValidationsException e)
            {
                // TODO: offer validation to end user by persisting it to an audit/log 

                var errors = e.Errors.Aggregate((curr, next) => $"{curr}; {next}");
                this._logger.LogError(e, errors);
            }
            catch (Exception e)
            { 
                this._logger.LogError(e, $"Something went wrong in {nameof(NewUserAddedConsumer)}");
            }
        }
    }
}

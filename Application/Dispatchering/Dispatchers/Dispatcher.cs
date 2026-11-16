using SystemctlPlugin.Application.Dispatchering.Commands;
using SystemctlPlugin.Application.Dispatchering.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace SystemctlPlugin.Application.Dispatchering.Dispatchers
{
    internal sealed class Dispatcher(IServiceProvider services) : IDispatcher
    {
        public TResult Execute<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>
            => services.GetRequiredService<ICommandHandler<TCommand, TResult>>().Handle(command);
    }
}

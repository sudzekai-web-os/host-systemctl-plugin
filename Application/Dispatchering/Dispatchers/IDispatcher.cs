using SystemctlPlugin.Application.Dispatchering.Commands;

namespace SystemctlPlugin.Application.Dispatchering.Dispatchers
{
    public interface IDispatcher
    {
        TResult Execute<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>;
    }
}

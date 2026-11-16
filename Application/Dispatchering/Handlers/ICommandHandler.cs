using SystemctlPlugin.Application.Dispatchering.Commands;

namespace SystemctlPlugin.Application.Dispatchering.Handlers
{
    internal interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
    {
        TResult Handle(TCommand command);
    }
}

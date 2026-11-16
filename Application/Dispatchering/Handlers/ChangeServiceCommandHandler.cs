using SystemctlPlugin.Application.Dispatchering.Commands;
using SystemctlPlugin.Application.Dto;
using SystemctlPlugin.Application.Services;

namespace SystemctlPlugin.Application.Dispatchering.Handlers
{
    internal sealed class ChangeServiceCommandHandler(SystemctlClient client) : ICommandHandler<ChangeServiceCommand, ServiceDetails>
    {
        public ServiceDetails Handle(ChangeServiceCommand command)
            => client.Change(command.Operation);
    }
}

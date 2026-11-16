using SystemctlPlugin.Application.Dispatchering.Commands;
using SystemctlPlugin.Application.Dto;
using SystemctlPlugin.Application.Services;

namespace SystemctlPlugin.Application.Dispatchering.Handlers
{
    internal sealed class AddTargetCommandHandler(SystemctlClient client) : ICommandHandler<AddTargetCommand, ServiceDetails>
    {
        public ServiceDetails Handle(AddTargetCommand command)
            => client.AddTarget(command.Name, command.Request.Target);
    }
}

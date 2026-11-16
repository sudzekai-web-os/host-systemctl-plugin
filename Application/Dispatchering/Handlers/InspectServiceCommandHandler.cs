using SystemctlPlugin.Application.Dispatchering.Commands;
using SystemctlPlugin.Application.Dto;
using SystemctlPlugin.Application.Services;

namespace SystemctlPlugin.Application.Dispatchering.Handlers
{
    internal sealed class InspectServiceCommandHandler(SystemctlClient client) : ICommandHandler<InspectServiceCommand, ServiceDetails>
    {
        public ServiceDetails Handle(InspectServiceCommand command)
            => client.Inspect(command.Name);
    }
}

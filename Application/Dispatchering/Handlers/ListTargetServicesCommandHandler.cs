using SystemctlPlugin.Application.Dispatchering.Commands;
using SystemctlPlugin.Application.Dto;
using SystemctlPlugin.Application.Services;

namespace SystemctlPlugin.Application.Dispatchering.Handlers
{
    internal sealed class ListTargetServicesCommandHandler(SystemctlClient client) : ICommandHandler<ListTargetServicesCommand, ServiceInfo[]>
    {
        public ServiceInfo[] Handle(ListTargetServicesCommand command)
            => client.ListTargetServices(command.Target);
    }
}

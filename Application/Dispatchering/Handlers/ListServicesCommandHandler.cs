using SystemctlPlugin.Application.Dispatchering.Commands;
using SystemctlPlugin.Application.Dto;
using SystemctlPlugin.Application.Services;

namespace SystemctlPlugin.Application.Dispatchering.Handlers
{
    internal sealed class ListServicesCommandHandler(SystemctlClient client) : ICommandHandler<ListServicesCommand, ServiceInfo[]>
    {
        public ServiceInfo[] Handle(ListServicesCommand command)
            => client.List();
    }
}

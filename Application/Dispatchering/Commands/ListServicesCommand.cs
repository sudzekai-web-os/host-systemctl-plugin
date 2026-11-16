using SystemctlPlugin.Application.Dto;

namespace SystemctlPlugin.Application.Dispatchering.Commands
{
    public sealed record ListServicesCommand : ICommand<ServiceInfo[]>;
}

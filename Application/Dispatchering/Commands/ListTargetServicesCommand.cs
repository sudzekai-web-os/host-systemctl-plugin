using SystemctlPlugin.Application.Dto;
namespace SystemctlPlugin.Application.Dispatchering.Commands
{
    public sealed record ListTargetServicesCommand(string Target) : ICommand<ServiceInfo[]>;
}

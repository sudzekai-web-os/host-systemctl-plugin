using SystemctlPlugin.Application.Dto;

namespace SystemctlPlugin.Application.Dispatchering.Commands
{
    public sealed record ChangeServiceCommand(ServiceOperation Operation) : ICommand<ServiceDetails>;
}

using SystemctlPlugin.Application.Dto;

namespace SystemctlPlugin.Application.Dispatchering.Commands
{
    public sealed record AddTargetCommand(string Name, TargetRequest Request) : ICommand<ServiceDetails>;
}

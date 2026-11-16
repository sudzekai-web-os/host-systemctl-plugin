using SystemctlPlugin.Application.Dto;

namespace SystemctlPlugin.Application.Dispatchering.Commands
{
    public sealed record ListTargetsCommand : ICommand<string[]>;
}

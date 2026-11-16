using SystemctlPlugin.Application.Dto;

namespace SystemctlPlugin.Application.Dispatchering.Commands
{
    public sealed record ReadUnitFileCommand(string Name) : ICommand<string>;
}

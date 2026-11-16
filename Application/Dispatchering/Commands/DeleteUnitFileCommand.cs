using SystemctlPlugin.Application.Dto;

namespace SystemctlPlugin.Application.Dispatchering.Commands
{
    public sealed record DeleteUnitFileCommand(string Name) : ICommand<bool>;
}

using SystemctlPlugin.Application.Dto;

namespace SystemctlPlugin.Application.Dispatchering.Commands
{
    public sealed record CreateUnitFileCommand(UnitFileRequest Request) : ICommand<ServiceDetails>;
}

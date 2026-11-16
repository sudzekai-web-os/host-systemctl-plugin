using SystemctlPlugin.Application.Dto;

namespace SystemctlPlugin.Application.Dispatchering.Commands
{
    public sealed record SaveUnitFileCommand(string Name, UnitFileRequest Request) : ICommand<ServiceDetails>;
}

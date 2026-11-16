using SystemctlPlugin.Application.Dispatchering.Commands;
using SystemctlPlugin.Application.Dto;
using SystemctlPlugin.Application.Services;

namespace SystemctlPlugin.Application.Dispatchering.Handlers
{
    internal sealed class ReadUnitFileCommandHandler(SystemctlClient client) : ICommandHandler<ReadUnitFileCommand, string>
    {
        public string Handle(ReadUnitFileCommand command)
            => client.ReadUnitFile(command.Name);
    }
}

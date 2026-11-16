using SystemctlPlugin.Application.Dispatchering.Commands;
using SystemctlPlugin.Application.Dto;
using SystemctlPlugin.Application.Services;

namespace SystemctlPlugin.Application.Dispatchering.Handlers
{
    internal sealed class DeleteUnitFileCommandHandler(SystemctlClient client) : ICommandHandler<DeleteUnitFileCommand, bool>
    {
        public bool Handle(DeleteUnitFileCommand command)
            => client.DeleteUnitFile(command.Name);
    }
}

using SystemctlPlugin.Application.Dispatchering.Commands;
using SystemctlPlugin.Application.Dto;
using SystemctlPlugin.Application.Services;

namespace SystemctlPlugin.Application.Dispatchering.Handlers
{
    internal sealed class SaveUnitFileCommandHandler(SystemctlClient client) : ICommandHandler<SaveUnitFileCommand, ServiceDetails>
    {
        public ServiceDetails Handle(SaveUnitFileCommand command)
            => client.SaveUnitFile(command.Name, command.Request.Content, create: false);
    }
}

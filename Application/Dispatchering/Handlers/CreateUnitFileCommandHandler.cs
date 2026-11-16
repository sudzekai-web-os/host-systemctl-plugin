using SystemctlPlugin.Application.Dispatchering.Commands;
using SystemctlPlugin.Application.Dto;
using SystemctlPlugin.Application.Services;

namespace SystemctlPlugin.Application.Dispatchering.Handlers
{
    internal sealed class CreateUnitFileCommandHandler(SystemctlClient client) : ICommandHandler<CreateUnitFileCommand, ServiceDetails>
    {
        public ServiceDetails Handle(CreateUnitFileCommand command)
            => client.SaveUnitFile(command.Request.Name, command.Request.Content, create: true);
    }
}

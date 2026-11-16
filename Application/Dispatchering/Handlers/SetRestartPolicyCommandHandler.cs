using SystemctlPlugin.Application.Dispatchering.Commands;
using SystemctlPlugin.Application.Dto;
using SystemctlPlugin.Application.Services;

namespace SystemctlPlugin.Application.Dispatchering.Handlers
{
    internal sealed class SetRestartPolicyCommandHandler(SystemctlClient client) : ICommandHandler<SetRestartPolicyCommand, ServiceDetails>
    {
        public ServiceDetails Handle(SetRestartPolicyCommand command)
            => client.SetRestartPolicy(command.Name, command.Request.Enabled);
    }
}

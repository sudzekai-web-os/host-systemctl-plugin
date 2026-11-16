using SystemctlPlugin.Application.Dto;

namespace SystemctlPlugin.Application.Dispatchering.Commands
{
    public sealed record SetRestartPolicyCommand(string Name, RestartPolicyRequest Request) : ICommand<ServiceDetails>;
}

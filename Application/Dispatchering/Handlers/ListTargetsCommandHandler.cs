using SystemctlPlugin.Application.Dispatchering.Commands;
using SystemctlPlugin.Application.Dto;
using SystemctlPlugin.Application.Services;

namespace SystemctlPlugin.Application.Dispatchering.Handlers
{
    internal sealed class ListTargetsCommandHandler(SystemctlClient client) : ICommandHandler<ListTargetsCommand, string[]>
    {
        public string[] Handle(ListTargetsCommand command)
            => client.ListTargets();
    }
}

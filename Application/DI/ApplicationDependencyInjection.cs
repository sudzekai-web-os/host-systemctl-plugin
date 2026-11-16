using Microsoft.Extensions.DependencyInjection;
using SystemctlPlugin.Application.Dispatchering.Commands;
using SystemctlPlugin.Application.Dispatchering.Dispatchers;
using SystemctlPlugin.Application.Dispatchering.Handlers;
using SystemctlPlugin.Application.Dto;
using SystemctlPlugin.Application.Services;

namespace SystemctlPlugin.Application.DI
{
    public static class ApplicationDependencyInjection
    {
        extension(IServiceCollection services)
        {
            public void AddApplicationServices()
            {
                services.AddSingleton<SystemctlClient>();
                services.AddSingleton<IDispatcher, Dispatcher>();
                services.AddTransient<ICommandHandler<ListTargetServicesCommand, ServiceInfo[]>, ListTargetServicesCommandHandler>();
                services.AddTransient<ICommandHandler<ListTargetsCommand, string[]>, ListTargetsCommandHandler>();
                services.AddTransient<ICommandHandler<ReadUnitFileCommand, string>, ReadUnitFileCommandHandler>();
                services.AddTransient<ICommandHandler<CreateUnitFileCommand, ServiceDetails>, CreateUnitFileCommandHandler>();
                services.AddTransient<ICommandHandler<SaveUnitFileCommand, ServiceDetails>, SaveUnitFileCommandHandler>();
                services.AddTransient<ICommandHandler<DeleteUnitFileCommand, bool>, DeleteUnitFileCommandHandler>();
                services.AddTransient<ICommandHandler<SetRestartPolicyCommand, ServiceDetails>, SetRestartPolicyCommandHandler>();
                services.AddTransient<ICommandHandler<AddTargetCommand, ServiceDetails>, AddTargetCommandHandler>();
                services.AddTransient<ICommandHandler<ListServicesCommand, ServiceInfo[]>, ListServicesCommandHandler>();
                services.AddTransient<ICommandHandler<InspectServiceCommand, ServiceDetails>, InspectServiceCommandHandler>();
                services.AddTransient<ICommandHandler<ChangeServiceCommand, ServiceDetails>, ChangeServiceCommandHandler>();
            }
        }
    }
}

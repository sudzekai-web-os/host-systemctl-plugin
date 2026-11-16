using Microsoft.Extensions.DependencyInjection;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Assembly;
using Sudzekai.LinCtrl.Core;
using SystemctlPlugin.Application.DI;

namespace SystemctlPlugin
{
    [PluginAssembly]
    public partial class SystemctlPlugin : IPluginAssembly
    {
        public void Configure(AssemblyHostBuilder builder)
        {
            LinCtrlContext.AddTypeInfoResolver(SystemctlJsonContext.Default);
            builder.Services.AddApplicationServices();
            builder.Services.AddTransient<IRequestHandler, ListTargetServicesHandler>();
            builder.Services.AddTransient<IRequestHandler, ListTargetsHandler>();
            builder.Services.AddTransient<IRequestHandler, ReadUnitFileHandler>();
            builder.Services.AddTransient<IRequestHandler, CreateUnitFileHandler>();
            builder.Services.AddTransient<IRequestHandler, SaveUnitFileHandler>();
            builder.Services.AddTransient<IRequestHandler, DeleteUnitFileHandler>();
            builder.Services.AddTransient<IRequestHandler, SetRestartPolicyHandler>();
            builder.Services.AddTransient<IRequestHandler, AddTargetHandler>();
            builder.Services.AddTransient<IRequestHandler, ListServicesHandler>();
            builder.Services.AddTransient<IRequestHandler, InspectServiceHandler>();
            builder.Services.AddTransient<IRequestHandler, StartServiceHandler>();
            builder.Services.AddTransient<IRequestHandler, StopServiceHandler>();
            builder.Services.AddTransient<IRequestHandler, RestartServiceHandler>();
            builder.Services.AddTransient<IRequestHandler, EnableServiceHandler>();
            builder.Services.AddTransient<IRequestHandler, DisableServiceHandler>();
        }
    }
}

using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;
using SystemctlPlugin.Application.Dispatchering.Commands;
using SystemctlPlugin.Application.Dispatchering.Dispatchers;
using SystemctlPlugin.Application.Dto;

namespace SystemctlPlugin
{
    internal sealed class ListTargetServicesHandler(IDispatcher dispatcher) : SystemctlRequestHandler
    {
        public override string Pattern => "systemctl/targets/{name}/services";

        public override HttpRequestMethod HttpMethod => HttpRequestMethod.Get;

        public override HandlerMetadata Metadata { get; } = new(
            string.Empty, "Список служб по target", ["systemctl"], "Список служб по target")
        {
            Parameters = [new("name", "path", SystemctlJsonContext.Default.String, "Имя target с суффиксом .target", required: true)],
            Responses = [new(200, LinCtrlContext.GetTypeInfo<ServiceInfo[]>()), new(400), new(401), new(403), new(404), new(409), new(413), new(415), new(503), new(504)]
        };

        protected override object Execute(HttpRequestDto request)
            => dispatcher.Execute<ListTargetServicesCommand, ServiceInfo[]>(new(Name(request)));
    }
}

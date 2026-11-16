using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;
using SystemctlPlugin.Application.Dispatchering.Commands;
using SystemctlPlugin.Application.Dispatchering.Dispatchers;
using SystemctlPlugin.Application.Dto;

namespace SystemctlPlugin
{
    internal sealed class StopServiceHandler(IDispatcher dispatcher) : SystemctlRequestHandler
    {
        public override string Pattern => "systemctl/services/{name}/stop";

        public override HttpRequestMethod HttpMethod => HttpRequestMethod.Post;

        public override HandlerMetadata Metadata { get; } = new(
            string.Empty, "Остановка службы", ["systemctl"], "Остановка службы")
        {
            Parameters = [new("name", "path", SystemctlJsonContext.Default.String, "Имя службы с суффиксом .service", required: true)],
            Responses = [new(200, LinCtrlContext.GetTypeInfo<ServiceDetails>()), new(400), new(401), new(403), new(404), new(409), new(503), new(504)]
        };

        protected override object Execute(HttpRequestDto request)
            => dispatcher.Execute<ChangeServiceCommand, ServiceDetails>(new(new(Name(request), "stop")));
    }
}

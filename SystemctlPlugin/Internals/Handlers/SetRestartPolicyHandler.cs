using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;
using SystemctlPlugin.Application.Dispatchering.Commands;
using SystemctlPlugin.Application.Dispatchering.Dispatchers;
using SystemctlPlugin.Application.Dto;

namespace SystemctlPlugin
{
    internal sealed class SetRestartPolicyHandler(IDispatcher dispatcher) : SystemctlRequestHandler
    {
        public override string Pattern => "systemctl/services/{name}/restart-policy";

        public override HttpRequestMethod HttpMethod => HttpRequestMethod.Put;

        public override HandlerMetadata Metadata { get; } = new(
            string.Empty, "Настройка авторестарта службы", ["systemctl"], "Настройка авторестарта службы")
        {
            Parameters = [new("name", "path", SystemctlJsonContext.Default.String, "Имя службы", required: true)],
            RequestBody = new(SystemctlJsonContext.Default.RestartPolicyRequest),
            Responses = [new(200, LinCtrlContext.GetTypeInfo<ServiceDetails>()), new(400), new(401), new(403), new(404), new(409), new(413), new(415), new(503), new(504)]
        };

        protected override object Execute(HttpRequestDto request)
            => dispatcher.Execute<SetRestartPolicyCommand, ServiceDetails>(new(Name(request), ReadBody(request, SystemctlJsonContext.Default.RestartPolicyRequest)));
    }
}

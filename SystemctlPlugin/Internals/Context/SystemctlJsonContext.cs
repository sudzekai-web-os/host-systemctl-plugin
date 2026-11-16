using System.Text.Json.Serialization;
using SystemctlPlugin.Application.Dto;

namespace SystemctlPlugin
{
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(string))]
    [JsonSerializable(typeof(string[]))]
    [JsonSerializable(typeof(bool))]
    [JsonSerializable(typeof(UnitFileRequest))]
    [JsonSerializable(typeof(RestartPolicyRequest))]
    [JsonSerializable(typeof(TargetRequest))]
    [JsonSerializable(typeof(ServiceInfo[]))]
    [JsonSerializable(typeof(ServiceDetails))]
    internal partial class SystemctlJsonContext : JsonSerializerContext
    {
    }
}

namespace SystemctlPlugin.Application.Dto
{
    public sealed record ServiceInfo(string Name, string LoadState, string ActiveState, string SubState, string Description);
}

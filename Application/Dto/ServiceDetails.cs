namespace SystemctlPlugin.Application.Dto
{
    public sealed record ServiceDetails(string Name, string Description, string LoadState, string ActiveState, string SubState, string UnitFileState, bool CanManage,
        bool CanEdit, string RestartPolicy, string[] Targets);
}

namespace Common.Integrations;

/// <summary>处理与给定模组的Integration。</summary>
internal interface IModIntegration
{
    /// <summary>可读的模组名称。</summary>
    public string Label { get; }

    /// <summary>该模组是否安装。</summary>
    public bool IsLoaded { get; }
}
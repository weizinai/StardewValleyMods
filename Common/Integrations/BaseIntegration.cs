namespace Common.Integrations;

internal class BaseIntegration : IModIntegration
{
    public string Label { get; }
    public bool IsLoaded { get; }
}
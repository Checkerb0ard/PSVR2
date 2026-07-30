namespace PSVR2.Features;

internal interface IFeature
{
    internal virtual void Initialize() { }
    internal virtual void OnUpdate() { }
}
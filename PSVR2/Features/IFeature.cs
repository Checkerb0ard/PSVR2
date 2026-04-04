namespace PSVR2.Features;

internal interface IFeature
{
    internal abstract void Initialize();
    internal virtual void OnUpdate() { }
}
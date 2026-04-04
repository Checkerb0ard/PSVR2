namespace PSVR2.Features;

internal class FeatureManager
{
    private IFeature adaptiveTriggers { get; set; }
    private IFeature fingerCurl { get; set; }
    
    internal FeatureManager()
    {
        adaptiveTriggers = new AdaptiveTriggers();
        fingerCurl = new FingerCurl();
        
        adaptiveTriggers.Initialize();
        fingerCurl.Initialize();
    }

    internal void OnUpdate()
    {
        adaptiveTriggers.OnUpdate();
        fingerCurl.OnUpdate();
    }
}
namespace PSVR2.Features;

internal class FeatureManager
{
    private IFeature adaptiveTriggers { get; set; }
    private IFeature headsetVibration { get; set; }
    private IFeature fingerCurl { get; set; }
    
    internal FeatureManager()
    {
        adaptiveTriggers = new AdaptiveTriggers();
        headsetVibration = new HeadsetVibration();
        fingerCurl = new FingerCurl();
        
        adaptiveTriggers.Initialize();
        headsetVibration.Initialize();
        fingerCurl.Initialize();
    }

    internal void OnUpdate()
    {
        adaptiveTriggers.OnUpdate();
        headsetVibration.OnUpdate();
        fingerCurl.OnUpdate();
    }
}
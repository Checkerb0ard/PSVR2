using BoneLib.BoneMenu;
using UnityEngine;

namespace PSVR2.BoneMenu;

internal class BoneMenuManager
{
    internal Page Page { get; private set; }
    
    internal Page AdaptiveTriggers { get; private set; }
    internal BoolElement AdaptiveTriggersEnabled { get; private set; }
    internal BoolElement SingleFireAdaptiveTriggersEnabled { get; private set; }
    internal IntElement SingleFireAdaptiveTriggersFeedback { get; private set; }
    internal BoolElement AutomaticAdaptiveTriggersEnabled { get; private set; }
    internal IntElement AutomaticAdaptiveTriggersFeedback { get; private set; }
    
    internal Page HeadsetVibration { get; private set; }
    internal BoolElement HeadsetVibrationEnabled { get; private set; }
    internal IntElement HeadsetVibrationStrength { get; private set; }
    
    internal BoneMenuManager()
    {
        var prefs = Core.Instance.PreferencesManager;
        
        Page = Page.Root.CreatePage("PSVR2", new Color32(0, 100, 220, 255));
        
        AdaptiveTriggers = Page.CreatePage("Adaptive Triggers", Color.white);
        AdaptiveTriggersEnabled = AdaptiveTriggersEnabled = AdaptiveTriggers.CreateBool("Enabled", Color.white, Core.Instance.PreferencesManager.AdaptiveTriggers.Value, b =>
        {
            prefs.AdaptiveTriggers.Value = b;
            prefs.Save();
        });
        SingleFireAdaptiveTriggersEnabled = AdaptiveTriggers.CreateBool("Single Fire", Color.white, Core.Instance.PreferencesManager.SingleFireAdaptiveTriggers.Value, b =>
        {
            prefs.SingleFireAdaptiveTriggers.Value = b;
            prefs.Save();
        });
        SingleFireAdaptiveTriggersFeedback = AdaptiveTriggers.CreateInt("Single Fire Feedback", Color.white, Core.Instance.PreferencesManager.SingleFireAdaptiveTriggersFeedback.Value, 1, 1, 8, i =>
        {
            prefs.SingleFireAdaptiveTriggersFeedback.Value = (byte)i;
            prefs.Save();
        });
        AutomaticAdaptiveTriggersEnabled = AdaptiveTriggers.CreateBool("Automatic", Color.white, Core.Instance.PreferencesManager.AutomaticAdaptiveTriggers.Value, b =>
        {
            prefs.AutomaticAdaptiveTriggers.Value = b;
            prefs.Save();
        });
        AutomaticAdaptiveTriggersFeedback = AdaptiveTriggers.CreateInt("Automatic Feedback", Color.white, Core.Instance.PreferencesManager.AutomaticAdaptiveTriggersFeedback.Value, 1, 1, 8, i =>
        {
            prefs.AutomaticAdaptiveTriggersFeedback.Value = (byte)i;
            prefs.Save();
        });
        
        HeadsetVibration = Page.CreatePage("Headset Vibration", Color.white);
        HeadsetVibrationEnabled = HeadsetVibration.CreateBool("Enabled", Color.white, Core.Instance.PreferencesManager.HeadsetVibration.Value, b =>
        {
            prefs.HeadsetVibration.Value = b;
            prefs.Save();
        });
        HeadsetVibrationStrength = HeadsetVibration.CreateInt("Headset Vibration Strength", Color.white, Core.Instance.PreferencesManager.HeadsetVibrationStrength.Value, 1, 1, 25, i =>
        {
            prefs.HeadsetVibrationStrength.Value = (byte)i;
            prefs.Save();
        });
    }
}
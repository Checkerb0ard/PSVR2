using MelonLoader;

namespace PSVR2.UserData;

internal class PreferencesManager
{
    private MelonPreferences_Category Category { get; set; }
    
    internal MelonPreferences_Entry<bool> AdaptiveTriggers;
    internal MelonPreferences_Entry<bool> SingleFireAdaptiveTriggers;
    internal MelonPreferences_Entry<byte> SingleFireAdaptiveTriggersFeedback;
    internal MelonPreferences_Entry<bool> AutomaticAdaptiveTriggers;
    internal MelonPreferences_Entry<byte> AutomaticAdaptiveTriggersFeedback;
    
    internal MelonPreferences_Entry<bool> HeadsetVibration;
    internal MelonPreferences_Entry<byte> HeadsetVibrationStrength;
    
    internal MelonPreferences_Entry<bool> FingerCurl;
    
    internal PreferencesManager()
    {
        Category = MelonPreferences.CreateCategory("PSVR2");
        
        AdaptiveTriggers = Category.CreateEntry("AdaptiveTriggers", true);
        SingleFireAdaptiveTriggers = Category.CreateEntry("SingleFireAdaptiveTriggers", true);
        SingleFireAdaptiveTriggersFeedback = Category.CreateEntry("SingleFireAdaptiveTriggersFeedback", (byte)4);
        AutomaticAdaptiveTriggers = Category.CreateEntry("AutomaticAdaptiveTriggers", true);
        AutomaticAdaptiveTriggersFeedback = Category.CreateEntry("AutomaticAdaptiveTriggersFeedback", (byte)8);
        
        HeadsetVibration = Category.CreateEntry("HeadsetVibration", true);
        HeadsetVibrationStrength = Category.CreateEntry("HeadsetVibrationStrength", (byte)16);
        
        FingerCurl = Category.CreateEntry("FingerCurl", true);
        
        Save();
    }
    
    internal void Save() => Category.SaveToFile(false);
}
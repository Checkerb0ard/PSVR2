using MelonLoader;

namespace PSVR2.UserData;

internal class PreferencesManager
{
    private MelonPreferences_Category Category { get; set; }
    
    internal MelonPreferences_Entry<bool> AdaptiveTriggers;
    internal MelonPreferences_Entry<byte> SingleFireFeedback;
    
    internal MelonPreferences_Entry<bool> HeadsetVibration;
    internal MelonPreferences_Entry<byte> HeadsetVibrationStrength;
    
    internal MelonPreferences_Entry<bool> FingerCurl;
    
    internal MelonPreferences_Entry<bool> EyeLidEstimation;
    
    internal PreferencesManager()
    {
        Category = MelonPreferences.CreateCategory("PSVR2");
        
        AdaptiveTriggers = Category.CreateEntry("AdaptiveTriggers", true);
        SingleFireFeedback = Category.CreateEntry("SingleFireFeedback", (byte)4);
        
        HeadsetVibration = Category.CreateEntry("HeadsetVibration", true);
        HeadsetVibrationStrength = Category.CreateEntry("HeadsetVibrationStrength", (byte)16);
        
        FingerCurl = Category.CreateEntry("FingerCurl", true);
        
        EyeLidEstimation = Category.CreateEntry("EyeLidEstimation", false);
        
        Save();
    }
    
    internal void Save() => Category.SaveToFile(false);
}
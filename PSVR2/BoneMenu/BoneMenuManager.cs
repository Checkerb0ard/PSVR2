using BoneLib.BoneMenu;
using PSVR2.Utilities;
using UnityEngine;

namespace PSVR2.BoneMenu;

internal class BoneMenuManager
{
    internal Page Page { get; private set; }
    
    internal BoolElement AdaptiveTriggers { get; private set; }
    
    internal BoolElement EyeLidEstimation { get; private set; }
    
    internal BoneMenuManager()
    {
        Page = Page.Root.CreatePage("PSVR2", new Color32(0, 100, 220, 255));
        
        AdaptiveTriggers = Page.CreateBool("Adaptive Triggers", Color.white, Core.Instance.PreferencesManager.AdaptiveTriggers.Value, b =>
        {
            var prefs = Core.Instance.PreferencesManager;
            
            prefs.AdaptiveTriggers.Value = b;
            prefs.Save();
        });

        if (MelonUtilities.HasEyeTracking())
        {
            EyeLidEstimation = Page.CreateBool("EyeLid Estimation", Color.white, Core.Instance.PreferencesManager.EyeLidEstimation.Value, b =>
            {
                var prefs = Core.Instance.PreferencesManager;
                
                prefs.EyeLidEstimation.Value = b;
                prefs.Save();
            });
            EyeLidEstimation.SetTooltip("Whether to use PSVR2Toolkit EyeLid estimation instead of binary blink for eye tracking. \n" +
                                        "Will only work if you have enabled EyeLid estimation in PSVR2Toolkit. \n" +
                                        "Please see their discord for instructions on how to enable this.");
        }
    }
}
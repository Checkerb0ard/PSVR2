using MelonLoader;
using PSVR2.BoneMenu;
using PSVR2.Features;
using PSVR2.PSVR2ToolKit;
using PSVR2.UserData;

[assembly: MelonInfo(typeof(PSVR2.Core), "PSVR2", "0.0.1", "Checkerboard")]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]
[assembly: MelonOptionalDependencies("EyeTracking")]

namespace PSVR2;

internal class Core : MelonMod
{
    internal static Core Instance { get; private set; }
    
    internal PreferencesManager PreferencesManager { get; private set; }
    internal PSVR2ToolKitManager ToolkitManager { get; private set; }
    internal FeatureManager FeatureManager { get; private set; }
    internal BoneMenuManager MenuManager { get; private set; }

    public override void OnInitializeMelon()
    {
        Instance = this;
        
        PreferencesManager = new PreferencesManager();
        ToolkitManager = new PSVR2ToolKitManager();
        FeatureManager = new FeatureManager();
        MenuManager = new BoneMenuManager();
    }

    public override void OnUpdate() => FeatureManager.OnUpdate();
}
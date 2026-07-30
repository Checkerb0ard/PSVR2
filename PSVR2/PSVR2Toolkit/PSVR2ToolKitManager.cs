using PSVR2Toolkit;

namespace PSVR2.PSVR2Toolkit;

internal class PSVR2ToolKitManager
{
    internal bool Loaded { get; private set; } = false;

    internal PSVR2ToolKitManager()
    {
        try
        {

            var result = PSVR2ToolkitCAPI.Init();
            Core.Instance.LoggerInstance.Msg("PSVR2ToolKit init result: " + result);
            if (result != 0)
            {
                Core.Instance.LoggerInstance.Error("PSVR2ToolKit failed to initialize.");
                Loaded = false;
                return;
            }
            
            Core.Instance.LoggerInstance.Msg("PSVR2ToolKit initialized successfully.");
            Loaded = true;
        }
        catch (Exception e)
        {
            Core.Instance.LoggerInstance.Error("Failed to start CAPI: " + e.Message);
        }
    }
}
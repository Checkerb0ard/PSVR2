using PSVR2Toolkit.CAPI;

namespace PSVR2.PSVR2ToolKit;

internal class PSVR2ToolKitManager
{
    internal IpcClient IpcClient { get; private set; } = IpcClient.Instance();
    internal bool Loaded { get; private set; } = false;

    internal PSVR2ToolKitManager()
    {
        try
        {
            Loaded = IpcClient.Start();
            
            if (!Loaded)
                Core.Instance.LoggerInstance.Error("Did you install the PSVR2 Toolkit Driver?");
        }
        catch (Exception e)
        {
            Core.Instance.LoggerInstance.Error("Failed to start IPC client: " + e.Message);
        }
    }
}
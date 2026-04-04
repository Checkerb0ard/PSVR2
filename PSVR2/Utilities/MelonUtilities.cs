using MelonLoader;

namespace PSVR2.Utilities;

internal static class MelonUtilities
{
    internal static bool HasEyeTracking()
    {
        return MelonBase.FindMelon("EyeTracking", "Checkerboard") != null;
    }
}
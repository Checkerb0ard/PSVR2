using BoneLib;
using HarmonyLib;
using Il2CppSLZ.Marrow;
using UnityEngine;

namespace PSVR2.Features;

internal class FingerCurl : IFeature
{
    public void Initialize()
    {
        
    }

    [HarmonyPatch(typeof(OpenController))]
    private static class OpenControllerPatches
    {
        [HarmonyPatch(nameof(OpenController.ProcessFingers))]
        [HarmonyPostfix]
        private static void ProcessFingers(OpenController __instance)
        {
            if (!Core.Instance.PreferencesManager.FingerCurl.Value)
                return;
            
            if (__instance.contRig.manager != Player.RigManager)
                return;

            __instance._processedMiddle = Mathf.Clamp(__instance._processedMiddle, 0.1f, 1f);
            __instance._processedIndex = Mathf.Clamp(__instance._processedIndex, 0.1f, 1f);
            __instance._processedRing = Mathf.Clamp(__instance._processedRing, 0.1f, 1f);
            __instance._processedPinky = Mathf.Clamp(__instance._processedPinky, 0.1f, 1f);
        }
    }
}
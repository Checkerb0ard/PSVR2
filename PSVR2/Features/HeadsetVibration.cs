using BoneLib;
using HarmonyLib;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Combat;
using PSVR2Toolkit;
using UnityEngine;

namespace PSVR2.Features;

internal class HeadsetVibration : IFeature
{
    private static bool _rumbleActive;
    private static float _rumbleEndTime;

    public void OnUpdate()
    {
        if (!_rumbleActive)
            return;

        if (Time.time >= _rumbleEndTime)
        {
            PSVR2ToolkitCAPI.SetHmdRumble(0);
            _rumbleActive = false;
        }
    }

    [HarmonyPatch(typeof(Player_Health))]
    private static class Player_HealthPatches
    {
        [HarmonyPatch(nameof(Player_Health.OnReceivedDamage))]
        [HarmonyPostfix]
        private static void OnReceivedDamage(Player_Health __instance, Attack attack)
        {
            if (!Core.Instance.PreferencesManager.HeadsetVibration.Value)
                return;

            if (__instance._rigManager != Player.RigManager)
                return;
            
            byte strength = Math.Clamp(Core.Instance.PreferencesManager.HeadsetVibrationStrength.Value, (byte)0, (byte)25);
            PSVR2ToolkitCAPI.SetHmdRumble(strength);

            _rumbleActive = true;
            _rumbleEndTime = Time.time + 0.5f;
        }
    }
}
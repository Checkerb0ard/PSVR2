#define DISABLELEFT

using BoneLib;
using HarmonyLib;
using Il2CppSLZ.Marrow;
using PSVR2Toolkit.CAPI;

namespace PSVR2.Features;

internal class AdaptiveTriggers : IFeature
{
    private enum FeedbackMode
    {
        None,
        Weapon,
        Vibration
    }

    internal static byte WeaponStart = 2;
    internal static byte WeaponEnd = 4;
    internal static byte WeaponStrength => Core.Instance.PreferencesManager.SingleFireFeedback.Value;
    internal static byte VibrationPosition = 4;
    internal static byte VibrationAmplitude = 8;

    public void Initialize()
    {
        Hooking.OnLevelLoaded += _ => TriggerManager.ForceDisableAll();
        Hooking.OnLevelUnloaded += TriggerManager.ForceDisableAll;
    }

    public void OnUpdate()
    {
        TriggerManager.Apply();
    }

    private class TriggerState
    {
        public FeedbackMode Mode = FeedbackMode.None;

        public byte Start, End, Strength;
        public byte Position, Amplitude, Frequency;

        public bool Dirty = true;
    }

    private static class TriggerManager
    {
        private static readonly Dictionary<EVRControllerType, TriggerState> States = new()
        {
            { EVRControllerType.Left, new TriggerState() },
            { EVRControllerType.Right, new TriggerState() }
        };

        internal static void SetNone(EVRControllerType controller, bool force = false)
        {
            var s = States[controller];
            if (!force && s.Mode == FeedbackMode.None) return;
        
            s.Mode = FeedbackMode.None;
            s.Dirty = true;
        }

        internal static void SetWeapon(EVRControllerType controller, byte start, byte end, byte strength)
        {
            var s = States[controller];

            if (s.Mode == FeedbackMode.Weapon &&
                s.Start == start && s.End == end && s.Strength == strength)
                return;

            s.Mode = FeedbackMode.Weapon;
            s.Start = start;
            s.End = end;
            s.Strength = strength;
            s.Dirty = true;
        }

        internal static void SetVibration(EVRControllerType controller, byte pos, byte amp, byte freq)
        {
            var s = States[controller];

            if (s.Mode == FeedbackMode.Vibration &&
                s.Position == pos && s.Amplitude == amp && s.Frequency == freq)
                return;

            s.Mode = FeedbackMode.Vibration;
            s.Position = pos;
            s.Amplitude = amp;
            s.Frequency = freq;
            s.Dirty = true;
        }

        internal static void Apply()
        {
            foreach (var (controller, s) in States)
            {
                if (!s.Dirty)
                    continue;

                try
                {
                    var ipc = Core.Instance.ToolkitManager.IpcClient;
                    
                    switch (s.Mode)
                    {
                        case FeedbackMode.None:
                            ipc.TriggerEffectDisable(controller);
                            break;

                        case FeedbackMode.Weapon:
                            ipc.TriggerEffectWeapon(controller, s.Start, s.End, s.Strength);
                            break;

                        case FeedbackMode.Vibration:
                            ipc.TriggerEffectVibration(controller, s.Position, s.Amplitude, s.Frequency);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Core.Instance.LoggerInstance.Error($"Trigger IPC failed: {e.Message}");
                }

                s.Dirty = false;
            }
        }

        internal static void ForceDisableAll()
        {
            SetNone(EVRControllerType.Left, true);
            SetNone(EVRControllerType.Right, true);
        }
    }

    [HarmonyPatch(typeof(Gun))]
    private static class GunPatches
    {
        [HarmonyPatch(nameof(Gun.Update))]
        [HarmonyPostfix]
        private static void Update(Gun __instance)
        {
            if (!Core.Instance.PreferencesManager.AdaptiveTriggers.Value)
                return;

            Hand hand = __instance.triggerGrip?.GetHand();

            if (hand == null)
                return;

            EVRControllerType controller;

            if (hand == Player.LeftHand)
            {
                #if DISABLELEFT
                return;
                #else
                controller = EVRControllerType.Left;
                #endif
            }
            else if (hand == Player.RightHand)
            {
                controller = EVRControllerType.Right;
            }
            else
            {
                return;
            }

            if (__instance._magState == null ||
                __instance._ammoInventory == null ||
                __instance.AmmoCount() <= 0 ||
                __instance.chamberedCartridge == null)
            {
                TriggerManager.SetNone(controller);
                return;
            }

            switch (__instance.fireMode)
            {
                case Gun.FireMode.MANUAL:
                case Gun.FireMode.SEMIAUTOMATIC:
                    TriggerManager.SetWeapon(controller,
                        WeaponStart,
                        WeaponEnd,
                        WeaponStrength);
                    break;

                case Gun.FireMode.AUTOMATIC:
                    byte freq = Convert.ToByte(Math.Min(40,
                        (int)Math.Round(__instance.roundsPerMinute / 60)));

                    TriggerManager.SetVibration(controller,
                        VibrationPosition,
                        VibrationAmplitude,
                        freq);
                    break;
            }
        }

        [HarmonyPatch(nameof(Gun.OnTriggerGripDetached))]
        [HarmonyPrefix]
        private static void OnTriggerGripDetached(Gun __instance, Hand hand)
        {
            if (!Core.Instance.PreferencesManager.AdaptiveTriggers.Value)
                return;
            
            if (hand == null)
                return;
            
            EVRControllerType controller;

            if (hand == Player.LeftHand)
            {
                controller = EVRControllerType.Left;
            }
            else if (hand == Player.RightHand)
            {
                controller = EVRControllerType.Right;
            }
            else
            {
                return;
            }
            
            TriggerManager.SetNone(controller);
        }
    }
}
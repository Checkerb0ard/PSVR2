using BoneLib;
using HarmonyLib;
using Il2CppSLZ.Marrow;
using PSVR2Toolkit;

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
        private static readonly Dictionary<VRControllerType, TriggerState> States = new()
        {
            { VRControllerType.Left, new TriggerState() },
            { VRControllerType.Right, new TriggerState() }
        };

        internal static void SetNone(VRControllerType controller, bool force = false)
        {
            var s = States[controller];
            if (!force && s.Mode == FeedbackMode.None) return;
        
            s.Mode = FeedbackMode.None;
            s.Dirty = true;
        }

        internal static void SetWeapon(VRControllerType controller, byte start, byte end, byte strength)
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

        internal static void SetVibration(VRControllerType controller, byte pos, byte amp, byte freq)
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
                    var command = new ScePadTriggerEffectCommand();
                    
                    switch (s.Mode)
                    {
                        case FeedbackMode.None:
                        {
                            command.mode = ScePadTriggerEffectMode.SCE_PAD_TRIGGER_EFFECT_MODE_OFF;
                            PSVR2ToolkitCAPI.SetTriggerEffect(controller, ref command);
                            break;
                        }

                        case FeedbackMode.Weapon:
                        {
                            command.mode = ScePadTriggerEffectMode.SCE_PAD_TRIGGER_EFFECT_MODE_WEAPON;
                            command.commandData.weaponStartPosition = s.Start;
                            command.commandData.weaponEndPosition = s.End;
                            command.commandData.weaponStrength = s.Strength;
                            PSVR2ToolkitCAPI.SetTriggerEffect(controller, ref command);
                            break;
                        }

                        case FeedbackMode.Vibration:
                        {
                            command.mode = ScePadTriggerEffectMode.SCE_PAD_TRIGGER_EFFECT_MODE_VIBRATION;
                            command.commandData.vibrationPosition = s.Position;
                            command.commandData.vibrationAmplitude = s.Amplitude;
                            command.commandData.vibrationFrequency = s.Frequency;
                            PSVR2ToolkitCAPI.SetTriggerEffect(controller, ref command);
                            break;
                        }
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
            SetNone(VRControllerType.Left, true);
            SetNone(VRControllerType.Right, true);
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

            VRControllerType controller;

            if (hand == Player.LeftHand)
            {
                controller = VRControllerType.Left;
            }
            else if (hand == Player.RightHand)
            {
                controller = VRControllerType.Right;
            }
            else
            {
                return;
            }

            if (__instance._magState == null ||
                __instance._ammoInventory == null ||
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
            
            VRControllerType controller;

            if (hand == Player.LeftHand)
            {
                controller = VRControllerType.Left;
            }
            else if (hand == Player.RightHand)
            {
                controller = VRControllerType.Right;
            }
            else
            {
                return;
            }
            
            TriggerManager.SetNone(controller);
        }
    }
}
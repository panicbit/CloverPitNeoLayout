using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Panik;
using static MainMenuScript;
using static Panik.Controls;
using static Panik.Data.SettingsData;

namespace CloverPitNeoLayout;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        Harmony.CreateAndPatchAll(typeof(Plugin));
    }

    static bool neoIsActive()
    {
        if (Data.settings == null)
        {
            return false;
        }

        var keyboardLayout = Data.settings.KeyboardLayoutGet();

        return keyboardLayout == KeyboardLayout.keyboard_DVORAK;
    }

    [HarmonyPatch(typeof(Controls), nameof(KeyboardLayoutGetElement))]
    [HarmonyPrefix]
    static bool PatchKeyHandling(InputAction action, ref KeyboardElement __result)
    {
        if (!neoIsActive())
        {
            return true;
        }

        __result = MapInputActionToNeoLayout(action);

        if (__result == KeyboardElement.Undefined)
        {
            var text = "PatchKeyHandling(): action not handled: " + action;
            Logger.LogWarning(text);
        }

        return false;
    }

    static KeyboardElement MapInputActionToNeoLayout(InputAction action)
    {
        return action switch
        {
            InputAction.menuMoveUp => KeyboardElement.V,    // W
            InputAction.menuMoveDown => KeyboardElement.I,  // S
            InputAction.menuMoveRight => KeyboardElement.A, // D
            InputAction.menuMoveLeft => KeyboardElement.U,  // A
            InputAction.menuSelect => KeyboardElement.Return,
            InputAction.menuSelectNoMouse => KeyboardElement.Return,
            InputAction.menuBack => KeyboardElement.Esc,
            InputAction.menuAnswerYes => KeyboardElement.Return,
            InputAction.menuAnswerNo => KeyboardElement.Esc,
            InputAction.menuPause => KeyboardElement.Esc,
            InputAction.menuTabLeft => KeyboardElement.X,  // Q
            InputAction.menuTabRight => KeyboardElement.L, // E
            InputAction.menuSocialButton => KeyboardElement.W, // T
            InputAction.moveUp => KeyboardElement.V,    // W
            InputAction.moveDown => KeyboardElement.I,  // S
            InputAction.moveLeft => KeyboardElement.U,  // A
            InputAction.moveRight => KeyboardElement.A, // D
            _ => KeyboardElement.Undefined,
        };
    }

    [HarmonyPatch(typeof(Controls), nameof(KeyboardButton_HoldGet), [typeof(PlayerExt), typeof(KeyboardElement)])]
    [HarmonyPrefix]
    static void PatchRestartKey(PlayerExt player, ref KeyboardElement element)
    {
        if (!neoIsActive())
        {
            return;
        }

        if (element == KeyboardElement.R)
        {
            element = KeyboardElement.C;
        }
    }

    [HarmonyPatch(typeof(MainMenuScript), "OptionsUpdateText_Desktop")]
    [HarmonyPostfix]
    static void PatchSettingsLabel(MainMenuScript __instance, MenuIndex ___menuIndex)
    {
        if (!neoIsActive())
        {
            return;
        }

        if (___menuIndex != MenuIndex.settingsOthers)
        {
            return;
        }

        __instance.optionTexts[5].text = "Keyboard: NEO";
    }
}

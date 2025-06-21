using Exiled.API.Features.Core.UserSettings;
using UnityEngine;

namespace AlphaWarheadOverride
{
    public class ServerSettings
    {
        private static KeybindSetting overrideKeybind;

        public static void RegisterSettings()
        {
            overrideKeybind = new KeybindSetting(
                id: 8768,
                label: "Alpha Warhead Override",
                suggested: KeyCode.U,
                preventInteractionOnGUI: false,
                allowSpectatorTrigger: false,
                hintDescription: "SCP-079: Use to start the detonation of Alpha Warhead (Tier 5).",
                onChanged: (player, s) => EventHandlers.HandleOverrideKey(player, s)
            );
            SettingBase.Register(new[] { overrideKeybind });;
        }
        public static void UnRegisterSettings()
        {
            if (overrideKeybind != null)
                SettingBase.Unregister(settings: new[] { overrideKeybind });
        }
    }
}
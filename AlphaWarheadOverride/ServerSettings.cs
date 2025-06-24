using UserSettings.ServerSpecific;
using UnityEngine;

namespace AlphaWarheadOverride
{
    public class ServerSettings
    {
        public static SSKeybindSetting OverrideKeybind { get; private set; }

        public static void RegisterSettings()
        {
            OverrideKeybind = new SSKeybindSetting(
                id: 8768,
                label: "Alpha Warhead Override",
                suggestedKey: KeyCode.U,
                preventInteractionOnGui: false,
                hint: "SCP-079: Use to start the detonation of Alpha Warhead (Tier 5)."
            );
        }

        public static void UnRegisterSettings()
        {
            if (OverrideKeybind != null)
                return;
        }
    }
}
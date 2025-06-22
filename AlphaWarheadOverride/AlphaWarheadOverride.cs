using System;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;

namespace AlphaWarheadOverride
{
    public class Plugin : Plugin<Config, Translations>
    {
        public override string Name => "AlphaWarheadOverride";
        public override string Author => "Vretu";
        public override string Prefix => "AWO";
        public override Version Version => new Version(1, 1, 0);
        public override Version RequiredExiledVersion { get; } = new Version(9, 6, 0);
        public static Plugin Instance { get; private set; }
        public HeaderSetting SettingsHeader { get; set; } = new HeaderSetting("Alpha Warhead Override");

        public override void OnEnabled()
        {
            Instance = this;
            SettingBase.Register(new[] { SettingsHeader });
            EventHandlers.RegisterEvents();
            ServerSettings.RegisterSettings();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;
            SettingBase.Unregister(settings: new[] { SettingsHeader });
            EventHandlers.UnregisterEvents();
            ServerSettings.UnRegisterSettings();
            base.OnDisabled();
        }
    }
}
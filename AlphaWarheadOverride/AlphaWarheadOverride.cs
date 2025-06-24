using LabApi.Features;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using System;

namespace AlphaWarheadOverride
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "AlphaWarheadOverride";
        public override string Author => "Vretu";
        public override string Description => "Display extra hint like a timers and notifications.";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredApiVersion { get; } = new Version(LabApiProperties.CompiledVersion);
        public static Plugin Instance { get; private set; }
        public Translation Translation { get; private set; }
        //public HeaderSetting SettingsHeader { get; set; } = new HeaderSetting("Alpha Warhead Override");

        public override void Enable()
        {
            Instance = this;
            //SettingBase.Register(new[] { SettingsHeader });
            EventHandlers.RegisterEvents();
            ServerSettings.RegisterSettings();
        }

        public override void Disable()
        {
            Instance = null;
            //SettingBase.Unregister(settings: new[] { SettingsHeader });
            EventHandlers.UnregisterEvents();
            ServerSettings.UnRegisterSettings();
        }
        public override void LoadConfigs()
        {
            this.TryLoadConfig("translation.yml", out Translation translation);
            Translation = translation ?? new Translation();
        }
    }
}
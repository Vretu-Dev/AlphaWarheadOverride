using System.Linq;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using Exiled.Events.EventArgs.Warhead;
using Exiled.API.Features.Roles;
using PlayerRoles;
using MEC;
using UnityEngine;

namespace AlphaWarheadOverride
{
    public static class EventHandlers
    {
        private static Player scp079Detonated = null;
        private static readonly Dictionary<int, bool> KeyWasPressed = new();
        private static readonly Dictionary<int, float> WarheadCooldown = new();
        private static float WarheadCooldownDuration = Plugin.Instance.Config.WarheadCooldown;
        public static void RegisterEvents()
        {
            Exiled.Events.Handlers.Warhead.Starting += OnWarheadDetonationStarted;
            Exiled.Events.Handlers.Warhead.Detonated += OnWarheadDetonated;
            Exiled.Events.Handlers.Warhead.Detonating += OnWarheadDetonating;
        }

        public static void UnregisterEvents()
        {
            Exiled.Events.Handlers.Warhead.Starting -= OnWarheadDetonationStarted;
            Exiled.Events.Handlers.Warhead.Detonated -= OnWarheadDetonated;
            Exiled.Events.Handlers.Warhead.Detonating -= OnWarheadDetonating;
        }

        public static void HandleOverrideKey(Player player, SettingBase setting)
        {
            if (setting is not KeybindSetting keybind)
                return;

            bool isPressed = keybind.IsPressed;
            bool wasPressed = KeyWasPressed.TryGetValue(player.Id, out bool prev) && prev;

            if (isPressed && !wasPressed)
            {
                TryStartDetonation(player);
            }

            KeyWasPressed[player.Id] = isPressed;
        }

        private static void TryStartDetonation(Player player)
        {
            if (player.Role is not Scp079Role scp079 || scp079.TierManager.AccessTierLevel != 5 || !player.IsAlive)
                return;

            if (scp079.Camera.Zone != ZoneType.Surface)
            {
                player.ShowHint(Plugin.Instance.Translation.NotOnSurface, 3f);
                return;
            }

            float now = Time.time;

            if (WarheadCooldown.TryGetValue(player.Id, out float cooldownUntil) && now < cooldownUntil)
            {
                float left = cooldownUntil - now;
                player.ShowHint(Plugin.Instance.Translation.WarheadCooldown.Replace("{seconds}", left.ToString("F0")), 3f);
                return;
            }

            if (Warhead.IsDetonated || Warhead.IsInProgress)
            {
                player.ShowHint(Plugin.Instance.Translation.AlreadyStartedOrDetonated, 3f);
                return;
            }

            Warhead.Start();
            scp079Detonated = player;
            WarheadCooldown[player.Id] = now + WarheadCooldownDuration;

            Map.Broadcast(5, Plugin.Instance.Translation.AlphaOverrideBroadcast);
        }

        private static void OnWarheadDetonationStarted(StartingEventArgs ev)
        {
            if (scp079Detonated != null && scp079Detonated.IsAlive)
                scp079Detonated.ShowHint(Plugin.Instance.Translation.StartedWarheadSequence, 5f);
        }

        private static void OnWarheadDetonating(DetonatingEventArgs ev)
        {
            if (scp079Detonated != null && scp079Detonated.IsAlive)
                scp079Detonated.IsGodModeEnabled = true;
        }

        private static void OnWarheadDetonated()
        {
            if (scp079Detonated != null && scp079Detonated.IsAlive)
            {
                scp079Detonated.IsGodModeEnabled = false;
                
                Timing.CallDelayed(2f, () =>
                {
                    bool anyOtherScpOnSurface = Player.List.Any(p => p.Role.Team == Team.SCPs && p.IsAlive && p.CurrentRoom.Zone == ZoneType.Surface && p != scp079Detonated);

                    if (!anyOtherScpOnSurface)
                    {
                        scp079Detonated.Kill(DamageType.Warhead);
                    }
                });
                
            }
            scp079Detonated = null;
        }
    }
}
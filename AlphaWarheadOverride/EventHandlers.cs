using System.Linq;
using System.Collections.Generic;
using PlayerRoles;
using MEC;
using UnityEngine;
using UserSettings.ServerSpecific;
using LabApi.Features.Wrappers;
using LabApi.Events.Arguments.WarheadEvents;
using MapGeneration;
using PlayerStatsSystem;
using PlayerRoles.PlayableScps.Scp079;

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
            LabApi.Events.Handlers.WarheadEvents.Starting += OnWarheadDetonationStarted;
            LabApi.Events.Handlers.WarheadEvents.Detonated += OnWarheadDetonated;
            LabApi.Events.Handlers.WarheadEvents.Detonating += OnWarheadDetonating;
        }

        public static void UnregisterEvents()
        {
            LabApi.Events.Handlers.WarheadEvents.Starting -= OnWarheadDetonationStarted;
            LabApi.Events.Handlers.WarheadEvents.Detonated -= OnWarheadDetonated;
            LabApi.Events.Handlers.WarheadEvents.Detonating -= OnWarheadDetonating;
        }

        public static void HandleOverrideKey(Player player, SSKeybindSetting keybind)
        {
            bool isPressed = keybind.SyncIsPressed;
            bool wasPressed = KeyWasPressed.TryGetValue(player.PlayerId, out bool prev) && prev;

            if (isPressed && !wasPressed)
            {
                TryStartDetonation(player);
            }

            KeyWasPressed[player.PlayerId] = isPressed;
        }

        private static void TryStartDetonation(Player player)
        {
            if (player.RoleBase is not Scp079Role scp079 || !scp079.SubroutineModule.TryGetSubroutine<Scp079TierManager>(out var tierManager) || tierManager.AccessTierLevel != 5 || !player.IsAlive)
                return;

            if (Room.GetRoomAtPosition(scp079.CurrentCamera.Position)?.Zone != FacilityZone.Surface)
            {
                player.SendHint(Plugin.Instance.Translation.NotOnSurface, 3f);
                return;
            }

            float now = Time.time;

            if (WarheadCooldown.TryGetValue(player.PlayerId, out float cooldownUntil) && now < cooldownUntil)
            {
                float left = cooldownUntil - now;
                player.SendHint(Plugin.Instance.Translation.WarheadCooldown.Replace("{seconds}", left.ToString("F0")), 3f);
                return;
            }

            if (Warhead.IsDetonated || Warhead.IsDetonationInProgress)
            {
                player.SendHint(Plugin.Instance.Translation.AlreadyStartedOrDetonated, 3f);
                return;
            }

            Warhead.Start();
            scp079Detonated = player;
            WarheadCooldown[player.PlayerId] = now + WarheadCooldownDuration;

            foreach (var players in Player.List.Where(p => p.IsAlive))
            {
                if (!players.IsHost)
                    players.SendBroadcast(Plugin.Instance.Translation.AlphaOverrideBroadcast, 10);
            }
        }

        private static void OnWarheadDetonationStarted(WarheadStartingEventArgs ev)
        {
            if (scp079Detonated != null && scp079Detonated.IsAlive)
                scp079Detonated.SendHint(Plugin.Instance.Translation.StartedWarheadSequence, 5f);
        }

        private static void OnWarheadDetonating(WarheadDetonatingEventArgs ev)
        {
            if (scp079Detonated != null && scp079Detonated.IsAlive)
                scp079Detonated.IsGodModeEnabled = true;
        }

        private static void OnWarheadDetonated(WarheadDetonatedEventArgs ev)
        {
            if (scp079Detonated != null && scp079Detonated.IsAlive)
            {
                scp079Detonated.IsGodModeEnabled = false;
                
                Timing.CallDelayed(2f, () =>
                {
                    bool anyOtherScpOnSurface = Player.List.Any(p => p.Team == Team.SCPs && p.IsAlive && p.Zone == FacilityZone.Surface && p != scp079Detonated);

                    if (!anyOtherScpOnSurface)
                    {
                        scp079Detonated.Kill();
                    }
                });
                
            }
            scp079Detonated = null;
        }
    }
}
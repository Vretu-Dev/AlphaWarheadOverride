namespace AlphaWarheadOverride
{
    public class Translation
    {
        public string AlphaOverrideBroadcast { get; set; } = "<color=red>SCP-079 has initiated the detonation of Alpha Warhead!</color>";
        public string AlreadyStartedOrDetonated { get; set; } = "Alpha Warhead has already been started or detonated!";
        public string StartedWarheadSequence { get; set; } = "You are immune to Alpha Warhead because you started it yourself.";
        public string NotOnSurface { get; set; } = "You can only start the Alpha Warhead detonation from the surface zone!";
        public string WarheadCooldown { get; set; } = "Alpha Warhead Override cooldown: {seconds}s.";
    }
}
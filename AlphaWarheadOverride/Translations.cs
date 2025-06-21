using Exiled.API.Interfaces;

namespace AlphaWarheadOverride
{
    public class Translations : ITranslation
    {
        public string AlphaOverrideBroadcast { get; set; } = "<color=red>SCP-079 has initiated the detonation of Alpha Warhead!</color>";
        public string AlreadyStartedOrDetonated { get; set; } = "Alpha Warhead has already been started or detonated!";
        public string StartedWarheadSequence { get; set; } = "You are immune to Alpha Warhead because you started it yourself.";
    }
}
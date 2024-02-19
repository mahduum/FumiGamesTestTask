using System;

namespace AbilitySystem.Scripts.GameplayEffects
{
    public enum GameplayEffectDuration
    {
        Instant,//no tags applied, change to base value
        Duration,//current value change, tags removed automatically when GE expires or is removed
        Infinite,//current value change, but tags must be removed explicitly
    }

    [Flags]
    public enum GameplayEffectCue
    {
        None = 0,
        PostProcessing = 1 << 0,
        Audio = 1 << 1
    }
}
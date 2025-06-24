using Content.Server._SSAF.Parasite.Components;
using Content.Server.Drunk;
using Content.Server.Speech.Components;
using Content.Shared.Drunk;
using Content.Shared.StatusEffect;

namespace Content.Server._SSAF.Parasite;

/// <summary>
/// This handles...
/// </summary>
public sealed class DrunkSpikeSystem : EntitySystem
{
    [ValidatePrototypeId<StatusEffectPrototype>]
    public const string DrunkSpikeKey = "DrunkSpike";

    [Dependency] private readonly DrunkSystem _drunk = default!;
    [Dependency] private readonly StatusEffectsSystem _statusEffects = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<DrunkSpikeComponent, ComponentAdd>(OnAdd);
        SubscribeLocalEvent<DrunkSpikeComponent, ComponentRemove>(OnRemoved);
    }

    private void OnAdd(EntityUid uid,
        DrunkSpikeComponent component,
        ComponentAdd args)
    {
        _statusEffects.TryAddStatusEffect<DrunkComponent>(uid, "Drunk", TimeSpan.FromMinutes(100), true);
        _statusEffects.TryAddStatusEffect<SlurredAccentComponent>(uid,
            "SlurredSpeech",
            TimeSpan.FromMinutes(100),
            true);
    }

    private void OnRemoved(EntityUid uid,
        DrunkSpikeComponent component,
        ComponentRemove args)
    {
        _statusEffects.TryRemoveStatusEffect(uid, "Drunk");
        _statusEffects.TryRemoveStatusEffect(uid, "SlurredSpeech");
    }


    public void TryDrunkSpike(EntityUid uid, TimeSpan time)
    {
        _statusEffects.TryAddStatusEffect<DrunkSpikeComponent>(uid, DrunkSpikeKey, time, true);
    }
}

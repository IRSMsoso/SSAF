using Content.Server.Explosion.EntitySystems;
using Content.Server.Ghost;
using Content.Server.Light.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Random;

namespace Content.Server._SSAF.Ghost;

/// <summary>
/// This handles...
/// </summary>
public sealed class GhostBooLightsTriggerSystem : EntitySystem
{
    [Dependency] private readonly TransformSystem _transform = default!;
    [Dependency] private readonly GhostSystem _ghost = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly IRobustRandom _random = default!;


    private readonly HashSet<Entity<PoweredLightComponent>> _lights = [];

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<GhostBooLightsOnTriggerComponent, TriggerEvent>(OnGhostBooLightsTrigger);


    }

    private void OnGhostBooLightsTrigger(EntityUid uid, GhostBooLightsOnTriggerComponent component, TriggerEvent args)
    {
        _lights.Clear();
        _lookup.GetEntitiesInRange<PoweredLightComponent>(Transform(uid).Coordinates, component.Range, _lights, LookupFlags.StaticSundries);
        foreach (var light in _lights) // static range of 5. because.
        {
            if (!_random.Prob(0.5f))
                continue;
            _ghost.DoGhostBooEvent(light);
        }
    }
}

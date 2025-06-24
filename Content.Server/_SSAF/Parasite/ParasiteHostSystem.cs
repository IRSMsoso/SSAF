using Content.Server._SSAF.Parasite.Components;
using Content.Server.Body.Components;
using Content.Server.Popups;
using Content.Shared._SSAF.Parasite;
using Content.Shared.Coordinates;
using Content.Shared.Popups;
using Robust.Server.Containers;
using Robust.Shared.Containers;

namespace Content.Server._SSAF.Parasite;

/// <summary>
/// This handles...
/// </summary>
public sealed class ParasiteHostSystem : EntitySystem
{

    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly ContainerSystem _container = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ParasiteHostComponent, EntRemovedFromContainerMessage>(OnEntRemovedFromContainer);
        SubscribeLocalEvent<ParasiteHostComponent, ComponentRemove>(OnComponentRemoved);
        SubscribeLocalEvent<ParasiteHostComponent, BeingGibbedEvent>(OnGibbed);
    }

    private void OnEntRemovedFromContainer(EntityUid uid,
        ParasiteHostComponent component,
        EntRemovedFromContainerMessage args)
    {
        // We only care about our parasite container
        if (component.ParasiteContainer != args.Container)
            return;

        EntityManager.RemoveComponent(uid, component);
    }

    private void CrawlOut(EntityUid uid, ParasiteHostComponent component)
    {
        if (component.ParasiteContainer.Count > 0)
        {
            var parasite = component.ParasiteContainer.ContainedEntities[0];
            _popup.PopupCoordinates("A grotesque looking worm crawls out", parasite.ToCoordinates(), PopupType.MediumCaution);
            RaiseLocalEvent(parasite, new ParasiteLoseHostEvent());
        }
        _container.EmptyContainer(component.ParasiteContainer);
    }

    private void OnComponentRemoved(EntityUid uid,
        ParasiteHostComponent component,
        ComponentRemove args)
    {
        CrawlOut(uid, component);
    }

    private void OnGibbed(EntityUid uid,
        ParasiteHostComponent component,
        BeingGibbedEvent args)
    {
        CrawlOut(uid, component);
    }
}

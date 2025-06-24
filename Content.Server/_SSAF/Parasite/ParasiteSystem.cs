using Content.Server._SSAF.Parasite.Components;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Shared._SSAF.Parasite;
using Content.Shared.Actions;
using Content.Shared.Coordinates;
using Content.Shared.DoAfter;
using Robust.Server.Containers;
using Robust.Shared.Containers;

namespace Content.Server._SSAF.Parasite;

/// <summary>
/// This handles...
/// </summary>
public sealed class ParasiteSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly DoAfterSystem _doAfter = default!;
    [Dependency] private readonly ContainerSystem _container = default!;

    private EntityQuery<ParasiteHostComponent> parasiteHostQuery;


    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ParasiteComponent, MapInitEvent>(OnInit);
        SubscribeLocalEvent<ParasiteComponent, ComponentRemove>(OnComponentRemove);
        SubscribeLocalEvent<ParasiteComponent, ParasiteInfectHostActionEvent>(OnInfectHost);
        SubscribeLocalEvent<ParasiteComponent, InfectHostDoAfterEvent>(OnDoAfterInfestHost);
    }

    private void OnComponentRemove(EntityUid uid, ParasiteComponent component, ComponentRemove args)
    {
        _container.TryRemoveFromContainer(uid);
    }

    private void OnInit(EntityUid uid, ParasiteComponent component, MapInitEvent args)
    {
        _actions.AddAction(uid, ref component.InfectHostActionEntity, component.InfectHostAction);
    }

    private void OnInfectHost(EntityUid uid, ParasiteComponent component, ParasiteInfectHostActionEvent args)
    {
        _popup.PopupEntity("You start worming your way under your target's skin", uid, uid);

        _doAfter.TryStartDoAfter(new DoAfterArgs(EntityManager,
            uid,
            component.InfectTime,
            new InfectHostDoAfterEvent(),
            uid,
            args.Target)
        {
            BreakOnMove = true,
        });
    }

    private void OnDoAfterInfestHost(EntityUid uid, ParasiteComponent component, InfectHostDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled)
            return;

        if (!args.Target.HasValue)
            return;

        var host = EnsureComp<ParasiteHostComponent>(args.Target.Value);

        host.ParasiteContainer = _container.EnsureContainer<Container>(args.Target.Value, "parasite");

        if (host.ParasiteContainer.Count > 0)
        {
            _popup.PopupEntity("Another has already snagged your prize...", uid, uid);
            return;
        }

        if (!_container.Insert(uid, host.ParasiteContainer))
        {
            _popup.PopupEntity("Failed", uid, uid);
            return;
        }

        _popup.PopupEntity("You worm your way into your new host", uid, uid);
    }
}

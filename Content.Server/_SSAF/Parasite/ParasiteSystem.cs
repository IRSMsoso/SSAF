using Content.Server._SSAF.Parasite.Components;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Shared._SSAF.Parasite;
using Content.Shared.Actions;
using Content.Shared.DoAfter;

namespace Content.Server._SSAF.Parasite;

/// <summary>
/// This handles...
/// </summary>
public sealed class ParasiteSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly DoAfterSystem _doAfter = default!;


    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ParasiteComponent, MapInitEvent>(OnInit);
        SubscribeLocalEvent<ParasiteComponent, ParasiteInfectHostActionEvent>(OnInfectHost);
        SubscribeLocalEvent<ParasiteComponent, InfectHostDoAfterEvent>(OnDoAfterInfestHost);
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

        _popup.PopupEntity("Success", uid, uid);
    }
}

using Content.Server._SSAF.Parasite.Components;
using Content.Server.Body.Systems;
using Content.Server.Chat.Systems;
using Content.Server.DoAfter;
using Content.Server.Drunk;
using Content.Server.Popups;
using Content.Server.Store.Systems;
using Content.Shared._Shitmed.Targeting;
using Content.Shared._SSAF.Parasite;
using Content.Shared.Actions;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Popups;
using Content.Shared.Store.Components;
using Robust.Server.Containers;
using Robust.Shared.Audio.Systems;
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
    [Dependency] private readonly DrunkSpikeSystem _drunkSpike = default!;
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly BloodstreamSystem _bloodstream = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly StoreSystem _store = default!;


    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ParasiteComponent, MapInitEvent>(OnInit);
        SubscribeLocalEvent<ParasiteComponent, ComponentRemove>(OnComponentRemove);

        SubscribeLocalEvent<ParasiteComponent, ParasiteInfectHostActionEvent>(OnInfectHost);
        SubscribeLocalEvent<ParasiteComponent, ParasiteMakeDrunkActionEvent>(OnMakeDrunk);
        SubscribeLocalEvent<ParasiteComponent, ParasiteEscapeActionEvent>(OnEscape);
        SubscribeLocalEvent<ParasiteComponent, ParasiteShopActionEvent>(OnShop);

        SubscribeLocalEvent<ParasiteComponent, InfectHostDoAfterEvent>(OnDoAfterInfestHost);
        SubscribeLocalEvent<ParasiteComponent, ParasiteEscapeDoAfterEvent>(OnDoAfterEscape);
        SubscribeLocalEvent<ParasiteComponent, ParasiteLoseHostEvent>(OnLoseHost);
    }

    private void OnComponentRemove(EntityUid uid, ParasiteComponent component, ComponentRemove args)
    {
        _container.TryRemoveFromContainer(uid);
    }

    private void OnInit(EntityUid uid, ParasiteComponent component, MapInitEvent args)
    {
        _actions.AddAction(uid, ref component.InfectHostActionEntity, component.InfectHostAction);
        _actions.AddAction(uid, ref component.ShopActionEntity, component.ShopAction);

    }

    private void OnInfectHost(EntityUid uid, ParasiteComponent component, ParasiteInfectHostActionEvent args)
    {
        if (!_doAfter.TryStartDoAfter(new DoAfterArgs(EntityManager,
                uid,
                component.InfectTime,
                new InfectHostDoAfterEvent(),
                uid,
                args.Target)
            {
                BreakOnMove = true,
            }))
            return;

        _popup.PopupEntity("You start worming your way under your target's skin", uid, uid);
    }

    private void OnMakeDrunk(EntityUid uid, ParasiteComponent component, ParasiteMakeDrunkActionEvent args)
    {
        if (!_container.TryGetContainingContainer(uid, out var container))
            return;

        _drunkSpike.TryDrunkSpike(container.Owner, component.MakeDrunkTime);

        _popup.PopupEntity( "You feel woozy all of a sudden", container.Owner, container.Owner);
    }

    private void OnEscape(EntityUid uid, ParasiteComponent component, ParasiteEscapeActionEvent args)
    {
        if (!_container.TryGetContainingContainer(uid, out var container))
            return;


        if (!_doAfter.TryStartDoAfter(new DoAfterArgs(EntityManager,
                uid,
                component.EscapeTime,
                new ParasiteEscapeDoAfterEvent(),
                uid)))
            return;

        // TODO: Cooldown
        _popup.PopupEntity("You feel something thrashing inside you", container.Owner, container.Owner, PopupType.LargeCaution);

    }

    private void OnShop(EntityUid uid, ParasiteComponent component, ParasiteShopActionEvent args)
    {
        if (!TryComp<StoreComponent>(uid, out var store))
            return;

        _store.ToggleUi(args.Performer, uid, store);
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
        _actions.RemoveAction(component.InfectHostActionEntity);
        _actions.AddAction(uid, ref component.EscapeActionEntity, component.EscapeAction);
    }

    private void OnDoAfterEscape(EntityUid uid, ParasiteComponent component, ParasiteEscapeDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled)
            return;

        if (!_container.TryGetContainingContainer(uid, out var container))
            return;

        _damageable.TryChangeDamage(container.Owner, component.EscapeDamage, true, true, null, uid, false, false, 1f, TargetBodyPart.Torso);
        _bloodstream.TryModifyBleedAmount(container.Owner, component.EscapeBleed);
        _audio.PlayPvs(component.EscapeSound, container.Owner.ToCoordinates());
        _chat.TryEmoteWithChat(container.Owner, component.ScreamEmote);
        RemComp<ParasiteHostComponent>(container.Owner);
    }

    private void OnLoseHost(EntityUid uid, ParasiteComponent component, ParasiteLoseHostEvent args)
    {
        _actions.AddAction(uid, ref component.InfectHostActionEntity, component.InfectHostAction);
        _actions.RemoveAction(component.EscapeActionEntity);

        _actions.SetCooldown(component.InfectHostActionEntity, component.HostInfestCooldownTime);
    }
}

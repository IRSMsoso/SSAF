using Content.Server.Chat.Systems;
using Content.Server.IdentityManagement;
using Content.Server.Popups;
using Content.Server.Stunnable;
using Content.Shared._SSAF.Xeno;
using Content.Shared.Actions;
using Content.Shared.Bed.Sleep;
using Content.Shared.Emoting;
using Content.Shared.Eye.Blinding.Systems;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Robust.Server.Audio;

namespace Content.Server._SSAF.Xenos;

/// <summary>
/// This handles...
/// </summary>
public sealed class IncapacitatorSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly StunSystem _stun = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly AudioSystem _audio = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<IncapacitatorComponent, MapInitEvent>(OnInit);
        SubscribeLocalEvent<IncapacitatorComponent, ComponentShutdown>(OnShutdown);

        SubscribeLocalEvent<IncapacitatorComponent, IncapacitateActionEvent>(OnIncapacitate);
    }

    private void OnInit(EntityUid uid, IncapacitatorComponent component, MapInitEvent args)
    {
        _actions.AddAction(uid, ref component.IncapacitateActionEntity, component.IncapacitateAction);

    }

    private void OnShutdown(EntityUid uid, IncapacitatorComponent component, ComponentShutdown args)
    {
        _actions.RemoveAction(component.IncapacitateActionEntity);
    }

    private void OnIncapacitate(EntityUid uid, IncapacitatorComponent component, IncapacitateActionEvent args)
    {
        _popup.PopupEntity(Name(args.Target) + " goes limp", args.Target, PopupType.LargeCaution);
        _stun.TryParalyze(args.Target, TimeSpan.FromSeconds(8), true);
        _chat.TryEmoteWithoutChat(args.Target, component.GaspEmote, true);
        _audio.PlayPvs(component.AlienTalkSound, uid);
        _actions.SetCooldown(component.IncapacitateActionEntity, TimeSpan.FromMinutes(2));
    }
}

using Content.Server._SSAF.Parasite.Components;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Server.Chat.Systems;
using Content.Server.DoAfter;
using Content.Server.Drunk;
using Content.Server.Jittering;
using Content.Server.Medical;
using Content.Server.Popups;
using Content.Server.Speech.EntitySystems;
using Content.Server.Store.Systems;
using Content.Server.Stunnable;
using Content.Shared._Shitmed.Targeting;
using Content.Shared._SSAF.Parasite;
using Content.Shared.Actions;
using Content.Shared.Body.Systems;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Gibbing.Events;
using Content.Shared.Gibbing.Systems;
using Content.Shared.Movement.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.Store.Components;
using Robust.Server.Containers;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server._SSAF.Parasite;

public enum Emotion
{
    Anger,
    Fear,
    Bliss,
    Despair,
    Disgust,
    Emptiness,
    Confusion,
}

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
    [Dependency] private readonly VomitSystem _vomit = default!;
    [Dependency] private readonly StunSystem _stun = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly HungerSystem _hunger = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly JitteringSystem _jittering = default!;
    [Dependency] private readonly GibbingSystem _gibbing = default!;
    [Dependency] private readonly StutteringSystem _stuttering = default!;


    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ParasiteComponent, MapInitEvent>(OnInit);
        SubscribeLocalEvent<ParasiteComponent, ComponentRemove>(OnComponentRemove);

        SubscribeLocalEvent<ParasiteComponent, BeingGibbedEvent>(OnGibbed);

        SubscribeLocalEvent<ParasiteComponent, ParasiteInfectHostActionEvent>(OnInfectHost);
        SubscribeLocalEvent<ParasiteComponent, ParasiteMakeDrunkActionEvent>(OnMakeDrunk);
        SubscribeLocalEvent<ParasiteComponent, ParasiteEscapeActionEvent>(OnEscape);

        // Emotion actions
        SubscribeLocalEvent<ParasiteComponent, ParasiteAffectEmotionAngerActionEvent>(OnAnger);
        SubscribeLocalEvent<ParasiteComponent, ParasiteAffectEmotionFearActionEvent>(OnFear);
        SubscribeLocalEvent<ParasiteComponent, ParasiteAffectEmotionBlissActionEvent>(OnBliss);
        SubscribeLocalEvent<ParasiteComponent, ParasiteAffectEmotionDespairActionEvent>(OnDespair);
        SubscribeLocalEvent<ParasiteComponent, ParasiteAffectEmotionDisgustActionEvent>(OnDisgust);
        SubscribeLocalEvent<ParasiteComponent, ParasiteAffectEmotionEmptinessActionEvent>(OnEmptiness);
        SubscribeLocalEvent<ParasiteComponent, ParasiteAffectEmotionConfusionActionEvent>(OnConfusion);

        // SubscribeLocalEvent<ParasiteComponent, ParasiteShopActionEvent>(OnShop);

        SubscribeLocalEvent<ParasiteComponent, InfectHostDoAfterEvent>(OnDoAfterInfestHost);
        SubscribeLocalEvent<ParasiteComponent, ParasiteEscapeDoAfterEvent>(OnDoAfterEscape);
        SubscribeLocalEvent<ParasiteComponent, ParasiteLoseHostEvent>(OnLoseHost);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<ParasiteComponent>();
        while (query.MoveNext(out var uid, out var parasite))
        {
            if (_timing.CurTime < parasite.NextUpdateTime)
                continue;
            parasite.NextUpdateTime = _timing.CurTime + parasite.UpdateRate;

            if (!_container.TryGetContainingContainer(uid, out var container))
                continue;

            var previousHungerStolen = parasite.HungerStolen;

            StealHunger(uid, container.Owner, parasite);


            if (JustReachedValue(200f, previousHungerStolen, parasite.HungerStolen))
            {
                _popup.PopupEntity("Something feels wrong", container.Owner, container.Owner, PopupType.Medium);
            }

            if (JustReachedValue(400f, previousHungerStolen, parasite.HungerStolen))
            {
                _popup.PopupEntity("I've been feeling hungrier than usual", container.Owner, container.Owner, PopupType.Medium);
            }

            if (JustReachedValue(600f, previousHungerStolen, parasite.HungerStolen))
            {
                _popup.PopupEntity("I feel nauseous", container.Owner, container.Owner, PopupType.MediumCaution);
            }

            if (JustReachedValue(800f, previousHungerStolen, parasite.HungerStolen))
            {
                _popup.PopupEntity("I'm so hungry all of the time", container.Owner, container.Owner, PopupType.Medium);
                _popup.PopupEntity("You are halfway to emerging", uid, uid, PopupType.Large);
            }

            if (JustReachedValue(1000f, previousHungerStolen, parasite.HungerStolen))
            {
                _popup.PopupEntity("I haven't felt like myself", container.Owner, container.Owner, PopupType.Medium);
            }

            if (JustReachedValue(1200f, previousHungerStolen, parasite.HungerStolen))
            {
                _popup.PopupEntity("Something is definitely very wrong", container.Owner, container.Owner, PopupType.MediumCaution);
            }

            if (JustReachedValue(1400f, previousHungerStolen, parasite.HungerStolen))
            {
                _popup.PopupEntity("Something is inside of me", container.Owner, container.Owner, PopupType.LargeCaution);
                _popup.PopupEntity("You will emerge soon", uid, uid, PopupType.Large);
            }

            if (Between(parasite.HungerStolen, 1400f, 1450f))
            {
                ThoughtWithProbability("Oh god, my insides hurt", container.Owner, 0.1f, PopupType.LargeCaution);
            }

            if (Between(parasite.HungerStolen, 1450f, 1500))
            {
                _stun.TryParalyze(container.Owner, TimeSpan.FromSeconds(2f), true);
                _jittering.DoJitter(container.Owner, TimeSpan.FromSeconds(2f), true, 1.0f, 10.0f);
                _stuttering.DoStutter(container.Owner, TimeSpan.FromSeconds(2f), true);
                ThoughtWithProbability("Everything hurts", container.Owner, 0.2f, PopupType.LargeCaution);
                EmoteWithProbability(container.Owner, parasite.ScreamEmote, 0.2f);
            }

            if (Between(parasite.HungerStolen, 1400f, 1500))
            {
                _bloodstream.TryModifyBleedAmount(container.Owner, 10f);
            }

            if (parasite.HungerStolen >= 1500)
            {
                // Consume host
                _damageable.TryChangeDamage(container.Owner,
                    new DamageSpecifier()
                    {
                        DamageDict = new Dictionary<string, FixedPoint2>
                        {
                            { "Blunt", 800 },
                        }
                    },
                    true);
                _popup.PopupCoordinates("A parasite bursts out!", uid.ToCoordinates(), PopupType.LargeCaution);
            }
        }
    }

    private static bool Between(float number, float min, float max)
    {
        return number >= min && number <= max;
    }

    private static bool JustReachedValue(float value, float previous, float current)
    {
        return current >= value && previous < value;
    }

    private void ThoughtWithProbability(string message, EntityUid uid, float probability, PopupType popupType = PopupType.Medium)
    {
        if (_random.Prob(probability))
        {
            _popup.PopupEntity(message, uid, uid, popupType);
        }
    }

    private void EmoteWithProbability(EntityUid uid, ProtoId<EmotePrototype> emote, float probability)
    {
        if (_random.Prob(probability))
        {
            _chat.TryEmoteWithChat(uid, emote);
        }
    }

    private void StealHunger(EntityUid parasiteUid, EntityUid hostUid, ParasiteComponent parasite)
    {
        if (!TryComp<HungerComponent>(parasiteUid, out var parasiteHungerComp))
            return;

        if (!TryComp<HungerComponent>(hostUid, out var hostHungerComp))
            return;

        if (_hunger.GetHunger(parasiteHungerComp) >= 150.0f)
            return;

        if (_hunger.GetHunger(hostHungerComp) < 10.0f)
        {
            ThoughtWithProbability("You feel extremely hungry", hostUid, 0.1f);
            parasite.HungerStolen += 0.1f;
            return;
        }

        _hunger.ModifyHunger(hostUid, -1.0f, hostHungerComp);
        _hunger.ModifyHunger(parasiteUid, 1.0f, parasiteHungerComp);
        parasite.HungerStolen += 1.0f;
    }

    private void OnComponentRemove(EntityUid uid, ParasiteComponent component, ComponentRemove args)
    {
        _container.TryRemoveFromContainer(uid);
    }

    private void OnGibbed(EntityUid uid, ParasiteComponent component, BeingGibbedEvent args)
    {
        if (!_container.TryGetContainingContainer(uid, out var container))
            return;

        if (!HasComp<ParasiteHostComponent>(container.Owner))
            return;

        _popup.PopupCoordinates(Loc.GetString("parasite-gibbed-in-host", ("mob", container.Owner)),
            container.Owner.ToCoordinates(),
            PopupType.LargeCaution);

        _bloodstream.TryModifyBleedAmount(container.Owner, component.EscapeBleed);
        _vomit.Vomit(container.Owner);
    }

    private void OnInit(EntityUid uid, ParasiteComponent component, MapInitEvent args)
    {
        _actions.AddAction(uid, ref component.InfectHostActionEntity, component.InfectHostAction);
        // _actions.AddAction(uid, ref component.ShopActionEntity, component.ShopAction);

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

        _popup.PopupEntity( "You suddenly feel extremely intoxicated", container.Owner, container.Owner, PopupType.Medium);
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

        _actions.SetCooldown(component.EscapeActionEntity, component.EscapeCooldownTime);
        _popup.PopupEntity("You feel something thrashing inside you", container.Owner, container.Owner, PopupType.LargeCaution);

    }

    #region Emotions

    private void OnAnger(EntityUid uid, ParasiteComponent component, ParasiteAffectEmotionAngerActionEvent args)
    {
        SwitchEmotion(uid, component, Emotion.Anger);
    }

    private void OnFear(EntityUid uid, ParasiteComponent component, ParasiteAffectEmotionFearActionEvent args)
    {
        SwitchEmotion(uid, component, Emotion.Fear);
    }

    private void OnBliss(EntityUid uid, ParasiteComponent component, ParasiteAffectEmotionBlissActionEvent args)
    {
        SwitchEmotion(uid, component, Emotion.Bliss);
    }

    private void OnDespair(EntityUid uid, ParasiteComponent component, ParasiteAffectEmotionDespairActionEvent args)
    {
        SwitchEmotion(uid, component, Emotion.Despair);
    }

    private void OnDisgust(EntityUid uid, ParasiteComponent component, ParasiteAffectEmotionDisgustActionEvent args)
    {
        SwitchEmotion(uid, component, Emotion.Disgust);
    }

    private void OnEmptiness(EntityUid uid, ParasiteComponent component, ParasiteAffectEmotionEmptinessActionEvent args)
    {
        SwitchEmotion(uid, component, Emotion.Emptiness);
    }

    private void OnConfusion(EntityUid uid, ParasiteComponent component, ParasiteAffectEmotionConfusionActionEvent args)
    {
        _chat.TrySendInGameOOCMessage(uid, "You suddenly feel completely and utterly confused", InGameOOCChatType.Looc, false);
        SwitchEmotion(uid, component, Emotion.Confusion);
    }

    private void SwitchEmotion(EntityUid uid, ParasiteComponent component, Emotion newEmotion)
    {
        component.CurrentEmotion = newEmotion;
        component.AffectEmotionUntil = _timing.CurTime + TimeSpan.FromSeconds(50);
    }

    #endregion

    // private void OnShop(EntityUid uid, ParasiteComponent component, ParasiteShopActionEvent args)
    // {
    //     if (!TryComp<StoreComponent>(uid, out var store))
    //         return;
    //
    //     _store.ToggleUi(args.Performer, uid, store);
    // }

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
        _actions.AddAction(uid, ref component.AngerActionEntity, component.AngerAction);
        _actions.AddAction(uid, ref component.FearActionEntity, component.FearAction);
        _actions.AddAction(uid, ref component.BlissActionEntity, component.BlissAction);
        _actions.AddAction(uid, ref component.DespairActionEntity, component.DespairAction);
        _actions.AddAction(uid, ref component.DisgustActionEntity, component.DisgustAction);
        _actions.AddAction(uid, ref component.EmptinessActionEntity, component.EmptinessAction);
        _actions.AddAction(uid, ref component.ConfusionActionEntity, component.ConfusionAction);
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
        _stun.TryParalyze(container.Owner, component.EscapeHostParalyzeTime, true);
        RemComp<ParasiteHostComponent>(container.Owner);
    }

    private void OnLoseHost(EntityUid uid, ParasiteComponent component, ParasiteLoseHostEvent args)
    {
        _actions.AddAction(uid, ref component.InfectHostActionEntity, component.InfectHostAction);
        _actions.RemoveAction(component.EscapeActionEntity);
        _actions.RemoveAction(component.AngerActionEntity);
        _actions.RemoveAction(component.FearActionEntity);
        _actions.RemoveAction(component.BlissActionEntity);
        _actions.RemoveAction(component.DespairActionEntity);
        _actions.RemoveAction(component.DisgustActionEntity);
        _actions.RemoveAction(component.EmptinessActionEntity);
        _actions.RemoveAction(component.ConfusionActionEntity);

        _actions.SetCooldown(component.InfectHostActionEntity, component.HostInfestCooldownTime);

        component.HungerStolen = 0f;
    }
}

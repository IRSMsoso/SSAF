using Content.Shared.Chat.Prototypes;
using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server._SSAF.Parasite.Components;

/// <summary>
/// Component data for the parasite.
/// </summary>
[RegisterComponent]
[AutoGenerateComponentPause]
public sealed partial class ParasiteComponent : Component
{
    [DataField]
    public EntityUid? InfectHostActionEntity;

    [DataField]
    public EntProtoId InfectHostAction = "ActionParasiteInfectHost";

    [DataField]
    public EntityUid? EscapeActionEntity;

    [DataField]
    public EntProtoId EscapeAction = "ActionParasiteEscape";

    [DataField]
    public EntProtoId MakeDrunkAction = "ActionMakeDrunk";

    [DataField]
    public EntityUid? MakeDrunkActionEntity;

    // [DataField]
    // public EntProtoId ShopAction = "ActionParasiteOpenShop";
    //
    // [DataField, AutoNetworkedField]
    // public EntityUid? ShopActionEntity;

    [DataField]
    public TimeSpan InfectTime = TimeSpan.FromSeconds(5);

    [DataField]
    public TimeSpan EscapeTime = TimeSpan.FromSeconds(3);

    [DataField]
    public TimeSpan HostInfestCooldownTime = TimeSpan.FromSeconds(2); // 60

    [DataField]
    public TimeSpan EscapeCooldownTime = TimeSpan.FromSeconds(2); // 40

    [DataField]
    public TimeSpan MakeDrunkCooldownTime = TimeSpan.FromSeconds(2); // 480

    [DataField]
    public TimeSpan MakeDrunkTime = TimeSpan.FromSeconds(90);

    [DataField]
    public TimeSpan EscapeHostParalyzeTime = TimeSpan.FromSeconds(4);

    [DataField, AutoNetworkedField]
    public ProtoId<EmotePrototype> ScreamEmote = "Scream";

    [DataField]
    public DamageSpecifier EscapeDamage = new DamageSpecifier()
    {
        DamageDict = new()
        {
            { "Slash", 90 },
        }
    };

    [DataField("escapeBleed")]
    public float EscapeBleed = 10.0f; // Max is 10 I guess?

    [ViewVariables(VVAccess.ReadWrite), DataField("escapeSound")]
    public SoundSpecifier? EscapeSound =
        new SoundPathSpecifier("/Audio/Effects/gib2.ogg")
        {
            Params = AudioParams.Default.WithVolume(3f),
        };

    #region Sustenance Stealing

    /// <summary>
    /// The time when the hunger threshold will update next.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoPausedField]
    public TimeSpan NextUpdateTime;

    /// <summary>
    /// The time between each hunger threshold update.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public TimeSpan UpdateRate = TimeSpan.FromSeconds(1);

    [DataField("hungerStolen")]
    public float HungerStolen = 0.0f;

    #endregion

    #region Emotions

    #region Anger

    [DataField]
    public EntProtoId AngerAction = "ActionParasiteAffectEmotionAnger";

    [DataField]
    public EntityUid? AngerActionEntity;

    #endregion

    #region Fear

    [DataField]
    public EntProtoId FearAction = "ActionParasiteAffectEmotionFear";

    [DataField]
    public EntityUid? FearActionEntity;

    #endregion

    #region Bliss

    [DataField]
    public EntProtoId BlissAction = "ActionParasiteAffectEmotionBliss";

    [DataField]
    public EntityUid? BlissActionEntity;

    #endregion

    #region Despair

    [DataField]
    public EntProtoId DespairAction = "ActionParasiteAffectEmotionDespair";

    [DataField]
    public EntityUid? DespairActionEntity;

    #endregion

    #region Disgust

    [DataField]
    public EntProtoId DisgustAction = "ActionParasiteAffectEmotionDisgust";

    [DataField]
    public EntityUid? DisgustActionEntity;

    #endregion

    #region Emptiness

    [DataField]
    public EntProtoId EmptinessAction = "ActionParasiteAffectEmotionEmptiness";

    [DataField]
    public EntityUid? EmptinessActionEntity;

    #endregion

    #region Confusion

    [DataField]
    public EntProtoId ConfusionAction = "ActionParasiteAffectEmotionConfusion";

    [DataField]
    public EntityUid? ConfusionActionEntity;

    #endregion


    public Emotion CurrentEmotion;

    /// <summary>
    /// The time when the current emotional effect will end.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoPausedField]
    public TimeSpan AffectEmotionUntil;

    #endregion
}


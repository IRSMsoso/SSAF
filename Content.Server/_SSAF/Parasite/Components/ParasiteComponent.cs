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
    public EntProtoId MakeDrunkAction = "ActionParasiteMakeDrunk";

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
    public TimeSpan HostInfestCooldownTime = TimeSpan.FromSeconds(60); // 60

    [DataField]
    public TimeSpan EscapeCooldownTime = TimeSpan.FromSeconds(40); // 40

    [DataField]
    public TimeSpan MakeDrunkCooldownTime = TimeSpan.FromSeconds(480); // 480

    [DataField]
    public TimeSpan MakeDrunkTime = TimeSpan.FromSeconds(90);

    [DataField]
    public TimeSpan EscapeHostParalyzeTime = TimeSpan.FromSeconds(4);

    [DataField, AutoNetworkedField]
    public ProtoId<EmotePrototype> ScreamEmote = "Scream";

    [DataField, AutoNetworkedField]
    public ProtoId<EmotePrototype> CryingEmote = "Crying";

    [DataField]
    public DamageSpecifier EscapeDamage = new DamageSpecifier()
    {
        DamageDict = new()
        {
            { "Slash", 90 },
        }
    };

    [DataField]
    public float EscapeBleed = 10.0f; // Max is 10 I guess?

    [DataField]
    public SoundSpecifier? EscapeSound =
        new SoundPathSpecifier("/Audio/Effects/gib2.ogg")
        {
            Params = AudioParams.Default.WithVolume(3f),
        };

    [DataField]
    public SoundSpecifier? StartSound =
        new SoundPathSpecifier("/Audio/_SSAF/parasite_intro.ogg")
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

    public Dictionary<Emotion, string[]> _emotionThoughts = new Dictionary<Emotion, string[]>
    {
        { Emotion.Anger, ["I'm so angry", "I feel like I need to hit something", "God damnit", "FUCK", "Fuck everyone else"] },
        { Emotion.Fear, ["What was that?", "Something's watching me", "They're onto me", "What was that noise?", "Why is that there?", "I gotta get out of here", "They're gonna kill me", "They're coming"] },
        { Emotion.Bliss, ["Life is good", "I feel great", "I feel so at peace", "I am on a cloud", "Everything is so serene"] },
        { Emotion.Despair, ["Everyone hates me", "It's my fault", "I let everyone down", "The world is a terrible place", "I am so unbelievably sad", "I feel unwell"] },
        { Emotion.Disgust, ["I'm gonna be sick", "I'm gonna puke", "The world is spinning", "Don't touch me", "I feel so sick", "Everything is so gross"] },
        { Emotion.Emptiness, ["I can't feel anything", "The world feels still", "Nothing ever happens", "Nothing matters"] },
        { Emotion.Confusion, ["Where am I?", "Who am I?", "I can't remember", "What was I doing?", "I can't focus", "What day is it?", "Why am I here?", "Am I supposed to be here?"] },

    };

    public Emotion CurrentEmotion;

    /// <summary>
    /// The time when the current emotional effect will end.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoPausedField]
    public TimeSpan AffectEmotionUntil;

    [DataField]
    public TimeSpan AffectEmotionTime = TimeSpan.FromSeconds(80);

    [DataField]
    public TimeSpan AffectEmotionCooldown = TimeSpan.FromMinutes(8);

    public bool NotifiedEndEmotion = true;

    #endregion
}


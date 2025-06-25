using Content.Shared.Chat.Prototypes;
using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server._SSAF.Parasite.Components;

/// <summary>
/// Component data for the parasite.
/// </summary>
[RegisterComponent]
public sealed partial class ParasiteComponent : Component
{
    [DataField("infectHostActionEntity")]
    public EntityUid? InfectHostActionEntity;

    [DataField("infectHostAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string InfectHostAction = "ActionParasiteInfectHost";

    [DataField("escapeActionEntity")]
    public EntityUid? EscapeActionEntity;

    [DataField("escapeAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string EscapeAction = "ActionParasiteEscape";

    [DataField]
    public EntProtoId MakeDrunkAction = "ActionMakeDrunk";

    [DataField]
    public EntProtoId ShopAction = "ActionParasiteOpenShop";

    [DataField, AutoNetworkedField]
    public EntityUid? ShopActionEntity;

    [DataField("infestTime")]
    public TimeSpan InfectTime = TimeSpan.FromSeconds(5);

    [DataField("escapeTime")]
    public TimeSpan EscapeTime = TimeSpan.FromSeconds(3);

    [DataField("hostInfestCooldownTime")]
    public TimeSpan HostInfestCooldownTime = TimeSpan.FromSeconds(2); // 60

    [DataField("makeDrunkCooldownTime")]
    public TimeSpan MakeDrunkCooldownTime = TimeSpan.FromSeconds(2); // 480

    [DataField("makeDrunkTime")]
    public TimeSpan MakeDrunkTime = TimeSpan.FromSeconds(90);

    [DataField("integration")]
    public float Integration = 0.0f;

    [DataField, AutoNetworkedField]
    public ProtoId<EmotePrototype> ScreamEmote = "Scream";

    [DataField("escapeDamage")]
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
}


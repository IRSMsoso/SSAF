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
    public string InfectHostAction = "ActionInfectHost";

    [DataField("makeDrunkActionEntity")]
    public EntityUid? makeDrunkActionEntity;

    [DataField("makeDrunkAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string makeDrunkAction = "ActionMakeDrunk";

    [DataField("escapeActionEntity")]
    public EntityUid? escapeActionEntity;

    [DataField("escapeAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string escapeAction = "ActionEscape";

    [DataField("infestTime")]
    public TimeSpan InfectTime = TimeSpan.FromSeconds(5);

    [DataField("escapeTime")]
    public TimeSpan EscapeTime = TimeSpan.FromSeconds(3);

    [DataField("loseHostInfestCooldownTime")]
    public TimeSpan LoseHostInfestCooldownTime = TimeSpan.FromSeconds(2); // 60

    [DataField("makeDrunkTime")]
    public TimeSpan makeDrunkTime = TimeSpan.FromSeconds(90);

    [DataField("makeDrunkCooldownTime")]
    public TimeSpan makeDrunkCooldownTime = TimeSpan.FromSeconds(2); // 480

    [DataField("energy")]
    public float Energy = 0.0f;

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


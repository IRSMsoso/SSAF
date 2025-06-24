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

    [DataField("infestTime")]
    public TimeSpan InfectTime = TimeSpan.FromSeconds(5);

    [DataField("loseHostInfestCooldownTime")]
    public TimeSpan LoseHostInfestCooldownTime = TimeSpan.FromSeconds(60);

    [DataField("makeDrunkTime")]
    public TimeSpan makeDrunkTime = TimeSpan.FromSeconds(90);

    [DataField("makeDrunkCooldownTime")]
    public TimeSpan makeDrunkCooldownTime = TimeSpan.FromSeconds(2); // 480

    [DataField("energy")]
    public float Energy = 0.0f;
}


using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server._SSAF.Parasite.Components;

/// <summary>
/// Component data for the parasite.
/// </summary>
[RegisterComponent]
public sealed partial class ParasiteComponent : Component
{
    [DataField("spawnRiftActionEntity")]
    public EntityUid? InfectHostActionEntity;

    [DataField("infectHostAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string InfectHostAction = "ActionInfectHost";
}

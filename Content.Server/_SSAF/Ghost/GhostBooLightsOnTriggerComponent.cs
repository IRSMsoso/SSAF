namespace Content.Server._SSAF.Ghost;

/// <summary>
/// Upon being triggered will trigger ghost boo effect on nearby lights
/// </summary>
[RegisterComponent]
public sealed partial class GhostBooLightsOnTriggerComponent : Component
{
    [DataField("range"), ViewVariables(VVAccess.ReadWrite)]
    public float Range = 1.0f;
}

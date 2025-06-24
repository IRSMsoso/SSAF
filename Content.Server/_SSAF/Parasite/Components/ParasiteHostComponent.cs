using Robust.Shared.Containers;

namespace Content.Server._SSAF.Parasite.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class ParasiteHostComponent : Component
{
    [ViewVariables(VVAccess.ReadOnly)]
    public Container ParasiteContainer = default!;
}

using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._SSAF.Parasite;


public sealed partial class ParasiteInfectHostActionEvent : EntityTargetActionEvent
{
}

[Serializable, NetSerializable]
public sealed partial class InfectHostDoAfterEvent : SimpleDoAfterEvent { }

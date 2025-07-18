using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._SSAF.Parasite;


public sealed partial class ParasiteInfectHostActionEvent : EntityTargetActionEvent
{
}

public sealed partial class ParasiteMakeDrunkActionEvent : InstantActionEvent
{
}

public sealed partial class ParasiteEscapeActionEvent : InstantActionEvent
{
}

[Serializable, NetSerializable]
public sealed partial class ParasiteEscapeDoAfterEvent : SimpleDoAfterEvent
{
}

public sealed partial class ParasiteLoseHostEvent : EntityEventArgs
{
}

[Serializable, NetSerializable]
public sealed partial class InfectHostDoAfterEvent : SimpleDoAfterEvent { }


public sealed partial class ParasiteShopActionEvent : InstantActionEvent
{
}


public sealed partial class ParasiteAffectEmotionAngerActionEvent : InstantActionEvent
{
}

public sealed partial class ParasiteAffectEmotionFearActionEvent : InstantActionEvent
{
}

public sealed partial class ParasiteAffectEmotionBlissActionEvent : InstantActionEvent
{
}

public sealed partial class ParasiteAffectEmotionDespairActionEvent : InstantActionEvent
{
}

public sealed partial class ParasiteAffectEmotionDisgustActionEvent : InstantActionEvent
{
}

public sealed partial class ParasiteAffectEmotionEmptinessActionEvent : InstantActionEvent
{
}

public sealed partial class ParasiteAffectEmotionConfusionActionEvent : InstantActionEvent
{
}

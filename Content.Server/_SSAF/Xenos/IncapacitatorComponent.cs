using Content.Shared.Chat.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server._SSAF.Xenos;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class IncapacitatorComponent : Component
{
    [DataField]
    public EntityUid? IncapacitateActionEntity;

    [DataField]
    public EntProtoId IncapacitateAction = "ActionIncapacitate";

    [DataField]
    public ProtoId<EmotePrototype> GaspEmote = "Gasp";

    [DataField("turnOnFailSound")]
    public SoundSpecifier AlienTalkSound = new SoundPathSpecifier("/Audio/_RMC14/Voice/Xeno/alien_talk2.ogg");
}

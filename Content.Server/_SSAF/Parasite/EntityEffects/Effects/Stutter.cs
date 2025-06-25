using Content.Shared.EntityEffects;
using Content.Shared.Jittering;
using Content.Shared.Speech.EntitySystems;
using Robust.Shared.Prototypes;

namespace Content.Server._SSAF.Parasite;

/// <summary>
/// This handles...
/// </summary>
public sealed partial class Stutter : EntityEffect
{
    [DataField]
    public float Time = 2.0f;

    /// <remarks>
    ///     true - refresh stutter time,  false - accumulate stutter time
    /// </remarks>
    [DataField]
    public bool Refresh = true;

    public override void Effect(EntityEffectBaseArgs args)
    {
        var time = Time;
        if (args is EntityEffectReagentArgs reagentArgs)
            time *= reagentArgs.Scale.Float();

        args.EntityManager.EntitySysManager.GetEntitySystem<SharedStutteringSystem>()
            .DoStutter(args.TargetEntity, TimeSpan.FromSeconds(time), Refresh);
    }

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys) =>
        Loc.GetString("reagent-effect-guidebook-stuttering", ("chance", Probability));
}

using Content.Server._SSAF.Parasite.Components;
using Content.Server.Temperature.Components;
using Content.Shared.EntityEffects;
using Robust.Shared.Prototypes;

namespace Content.Server.EntityEffects.EffectConditions;

/// <summary>
///     Requires that the entity has a parasite
/// </summary>
public sealed partial class HasParasite : EntityEffectCondition
{
    [DataField]
    public bool Inverted = false;

    public override bool Condition(EntityEffectBaseArgs args)
    {
        var condition = args.EntityManager.HasComponent<ParasiteHostComponent>(args.TargetEntity);
        if (Inverted)
        {
            condition = !condition;
        }
        return condition;
    }

    public override string GuidebookExplanation(IPrototypeManager prototype)
    {
        return Loc.GetString(Inverted ? "reagent-effect-condition-guidebook-has-parasite-inverted" : "reagent-effect-condition-guidebook-has-parasite");
    }
}

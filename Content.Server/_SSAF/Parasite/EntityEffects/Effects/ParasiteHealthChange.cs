using System.Text.Json.Serialization;
using Content.Shared._Shitmed.Targeting;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Content.Shared.Localizations;
using Robust.Server.Containers;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;

namespace Content.Server._SSAF.Parasite;

/// <summary>
/// This handles...
/// </summary>
public sealed partial class ParasiteHealthChange : EntityEffect
{
    /// <summary>
    /// Damage to apply every cycle. Damage Ignores resistances.
    /// </summary>
    [DataField(required: true)]
    [JsonPropertyName("damage")]
    public DamageSpecifier Damage = default!;

    /// <summary>
    ///     Should this effect scale the damage by the amount of chemical in the solution?
    ///     Useful for touch reactions, like styptic powder or acid.
    ///     Only usable if the EntityEffectBaseArgs is an EntityEffectReagentArgs.
    /// </summary>
    [DataField]
    [JsonPropertyName("scaleByQuantity")]
    public bool ScaleByQuantity;

    [DataField]
    [JsonPropertyName("ignoreResistances")]
    public bool IgnoreResistances = true;

    protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        var damages = new List<string>();
        var heals = false;
        var deals = false;

        var damageSpec = new DamageSpecifier(Damage);

        var universalReagentDamageModifier = entSys.GetEntitySystem<DamageableSystem>().UniversalReagentDamageModifier;
        var universalReagentHealModifier = entSys.GetEntitySystem<DamageableSystem>().UniversalReagentHealModifier;

        if (universalReagentDamageModifier != 1 || universalReagentHealModifier != 1)
        {
            foreach (var (type, val) in damageSpec.DamageDict)
            {
                if (val < 0f)
                {
                    damageSpec.DamageDict[type] = val * universalReagentHealModifier;
                }
                if (val > 0f)
                {
                    damageSpec.DamageDict[type] = val * universalReagentDamageModifier;
                }
            }
        }

        damageSpec = entSys.GetEntitySystem<DamageableSystem>().ApplyUniversalAllModifiers(damageSpec);

        foreach (var (kind, amount) in damageSpec.DamageDict)
        {
            var sign = FixedPoint2.Sign(amount);

            if (sign < 0)
                heals = true;
            if (sign > 0)
                deals = true;

            damages.Add(
                Loc.GetString("health-change-parasite-display",
                    ("kind", prototype.Index<DamageTypePrototype>(kind).LocalizedName),
                    ("amount", MathF.Abs(amount.Float())),
                    ("deltasign", sign)
                ));
        }

        var healsordeals = heals ? (deals ? "both" : "heals") : (deals ? "deals" : "none");

        return Loc.GetString("reagent-effect-guidebook-health-change",
            ("chance", Probability),
            ("changes", ContentLocalizationManager.FormatList(damages)),
            ("healsordeals", healsordeals));
    }

    public override void Effect(EntityEffectBaseArgs args)
    {
        var scale = FixedPoint2.New(1);
        var damageSpec = new DamageSpecifier(Damage);

        if (args is EntityEffectReagentArgs reagentArgs)
        {
            scale = ScaleByQuantity ? reagentArgs.Quantity * reagentArgs.Scale : reagentArgs.Scale;
        }

        var universalReagentDamageModifier = args.EntityManager.System<DamageableSystem>().UniversalReagentDamageModifier;
        var universalReagentHealModifier = args.EntityManager.System<DamageableSystem>().UniversalReagentHealModifier;

        if (universalReagentDamageModifier != 1 || universalReagentHealModifier != 1)
        {
            foreach (var (type, val) in damageSpec.DamageDict)
            {
                if (val < 0f)
                {
                    damageSpec.DamageDict[type] = val * universalReagentHealModifier;
                }
                if (val > 0f)
                {
                    damageSpec.DamageDict[type] = val * universalReagentDamageModifier;
                }
            }
        }

        if (!args.EntityManager.System<ContainerSystem>().TryGetContainer(args.TargetEntity, "parasite", out var container))
            return;

        if (container.Count == 0)
            return;

        var parasite = container.ContainedEntities[0];

        args.EntityManager.System<DamageableSystem>()
            .TryChangeDamage(
                parasite,
                damageSpec * scale,
                IgnoreResistances,
                interruptsDoAfters: false,
                // Shitmed Change Start
                targetPart: TargetBodyPart.All,
                partMultiplier: 0.5f,
                canSever: false);
                // Shitmed Change End
    }
}

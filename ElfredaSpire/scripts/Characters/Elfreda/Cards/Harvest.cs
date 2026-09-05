// | 收割 | Harvest | 攻击 | 2 | 场上每存在一名敌方单位，对所有敌方单位造成6/8点伤害一次。对带有[花]的目标伤害翻倍。消耗。 |

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.ValueProps;
using ElfredaSpire.GeneralPowers;

namespace ElfredaSpire.Characters.Elfreda.Cards;

[RegisterCard(typeof(ElfredaCardPool))]
public class Harvest : ModCardTemplate
{
    public Harvest() : base(2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies, true)
    {
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<FlowerElfredaPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState is null) { return; }
        int hitCount = CombatState.HittableEnemies.Count;
        await using AttackContext attackContext = await AttackCommand.CreateContextAsync(CombatState, choiceContext, this);

        for (int i = 0; i < hitCount; i++)
        {
            List<DamageResult> hitResults = [];
            foreach (Creature target in CombatState.HittableEnemies)
            {
                decimal damage = DynamicVars.Damage.BaseValue;
                if (target.HasPower<FlowerElfredaPower>())
                {
                    damage *= 2;
                }

                hitResults.AddRange(await CreatureCmd.Damage(
                    choiceContext,
                    target,
                    damage,
                    DynamicVars.Damage.Props,
                    Owner.Creature,
                    this));
            }

            attackContext.AddHit(hitResults);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
}

// | 花海之怒 | FlowerSeaFury | 攻击 | 3 | 保留。对所有敌人造成12/16点伤害。移除所有敌人身上的所有[花]，每移除1层，对随机敌人造成3/4点伤害。消耗。 |

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using ElfredaSpire.GeneralPowers;

namespace ElfredaSpire.Characters.Elfreda.Cards;

[RegisterCard(typeof(ElfredaCardPool))]
public class FlowerSeaFury : ModCardTemplate
{
    public FlowerSeaFury() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Retain];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<FlowerElfredaPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(12, ValueProp.Move), new DamageVar("FlowerDamage", 3, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState is null) return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .Execute(choiceContext);

        int flowerAmount = 0;
        foreach (var enemy in CombatState.HittableEnemies.ToList())
        {
            FlowerElfredaPower? flower = enemy.GetPower<FlowerElfredaPower>();
            if (flower is null)
            {
                continue;
            }

            flowerAmount += flower.Amount;
            await PowerCmd.Remove(flower);
        }

        if (flowerAmount > 0)
        {
            await DamageCmd.Attack(DynamicVars["FlowerDamage"].BaseValue)
                .WithHitCount(flowerAmount)
                .FromCard(this)
                .TargetingRandomOpponents(CombatState)
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
        DynamicVars["FlowerDamage"].UpgradeValueBy(1);
    }
}

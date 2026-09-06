// | 花粉传播 | PollenSpread | 技能 | 2 | 移除目标一半的[花]。对所有其他敌人施加等同于移除量-/两倍的[花]。 |

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Commands;
using ElfredaSpire.GeneralPowers;

namespace ElfredaSpire.Characters.Elfreda.Cards;

[RegisterCard(typeof(ElfredaCardPool))]
public class PollenSpread : ModCardTemplate
{
    public PollenSpread() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<FlowerElfredaPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is null) return;
        if (CombatState is not { } combatState)
        {
            return;
        }

        FlowerElfredaPower? flower = cardPlay.Target.GetPower<FlowerElfredaPower>();
        if (flower is null)
        {
            return;
        }

        int flowerAmountBeforeRemoval = flower.Amount;
        int requestedRemoval = flowerAmountBeforeRemoval / 2;
        if (requestedRemoval <= 0)
        {
            return;
        }

        int flowerAmountAfterRemoval = await PowerCmd.ModifyAmount(
            choiceContext,
            flower,
            -requestedRemoval,
            Owner.Creature,
            cardPlay.Card);
        int removedAmount = flowerAmountBeforeRemoval - Math.Max(0, flowerAmountAfterRemoval);
        int amountToSpread = removedAmount * (IsUpgraded ? 2 : 1);

        foreach (Creature enemy in combatState.HittableEnemies)
        {
            if (enemy != cardPlay.Target)
            {
                await PowerCmd.Apply<FlowerElfredaPower>(
                    choiceContext,
                    enemy,
                    amountToSpread,
                    Owner.Creature,
                    cardPlay.Card);
            }
        }
    }
}

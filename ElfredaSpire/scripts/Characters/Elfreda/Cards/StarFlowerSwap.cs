// | 星花转换 | StarFlowerSwap | 技能 | 2 | 将自身的[星]层数与目标的[花]层数互换。消耗。/- |

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Commands;
using ElfredaSpire.GeneralPowers;

namespace ElfredaSpire.Characters.Elfreda.Cards;

[RegisterCard(typeof(ElfredaCardPool))]
public class StarFlowerSwap : ModCardTemplate
{
    public StarFlowerSwap() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<StarElfredaPower>(),
        HoverTipFactory.FromPower<FlowerElfredaPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is null) return;

        StarElfredaPower? star = Owner.Creature.GetPower<StarElfredaPower>();
        FlowerElfredaPower? flower = cardPlay.Target.GetPower<FlowerElfredaPower>();
        int starAmount = star?.Amount ?? 0;
        int flowerAmount = flower?.Amount ?? 0;

        await PowerCmd.Remove(star);
        await PowerCmd.Remove(flower);

        if (flowerAmount > 0)
        {
            await PowerCmd.Apply<FlowerElfredaPower>(choiceContext, Owner.Creature, flowerAmount, Owner.Creature, cardPlay.Card);
        }

        if (starAmount > 0)
        {
            await PowerCmd.Apply<StarElfredaPower>(choiceContext, cardPlay.Target, starAmount, Owner.Creature, cardPlay.Card);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}

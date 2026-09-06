// | 花海降临 | FlowerSeaArrival | 技能 | 1 | 对所有敌人施加5/8层[花]。抽2/3张牌。 |

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Commands;
using ElfredaSpire.GeneralPowers;

namespace ElfredaSpire.Characters.Elfreda.Cards;

[RegisterCard(typeof(ElfredaCardPool))]
public class FlowerSeaArrival : ModCardTemplate
{
    public FlowerSeaArrival() : base(1, CardType.Skill, CardRarity.Ancient, TargetType.AllEnemies, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<FlowerElfredaPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FlowerElfredaPower>(5), new CardsVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FlowerElfredaPower>(choiceContext, cardPlay.Target!, DynamicVars["FlowerElfredaPower"].IntValue, Owner.Creature, cardPlay.Card);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FlowerElfredaPower"].UpgradeValueBy(3);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
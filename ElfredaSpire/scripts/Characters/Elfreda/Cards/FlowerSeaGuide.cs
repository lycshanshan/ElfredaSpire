// | 花海引路 | FlowerSeaGuide | 技能 | 1 | 抽3/4张牌。对所有敌人施加2/3层虚弱和4/6层花。消耗。 |

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Commands;
using ElfredaSpire.GeneralPowers;

namespace ElfredaSpire.Characters.Elfreda.Cards;

[RegisterCard(typeof(ElfredaCardPool))]
public class FlowerSeaGuide : ModCardTemplate
{
    public FlowerSeaGuide() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies, true)
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

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<FlowerElfredaPower>(), HoverTipFactory.FromPower<WeakPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3), new PowerVar<FlowerElfredaPower>(4), new PowerVar<WeakPower>(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState is null) return;
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        await PowerCmd.Apply<FlowerElfredaPower>(choiceContext, CombatState.HittableEnemies.ToList(), DynamicVars["FlowerElfredaPower"].IntValue, Owner.Creature, cardPlay.Card);
        await PowerCmd.Apply<WeakPower>(choiceContext, CombatState.HittableEnemies.ToList(), DynamicVars["WeakPower"].IntValue, Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
        DynamicVars["FlowerElfredaPower"].UpgradeValueBy(2);
        DynamicVars["WeakPower"].UpgradeValueBy(1);
    }
}

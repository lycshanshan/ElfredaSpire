// | 开花 | Bloom | 技能 | 0 | 施加2/3层[花]。抽1/2张牌。|

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
[RegisterCharacterStarterCard(typeof(ElfredaCharacter), 1)]
public class Bloom : ModCardTemplate
{
    public Bloom() : base(0, CardType.Skill, CardRarity.Basic, TargetType.AnyEnemy, true)
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

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FlowerElfredaPower>(2), new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FlowerElfredaPower>(choiceContext, cardPlay.Target!, DynamicVars["FlowerElfredaPower"].IntValue, Owner.Creature, cardPlay.Card);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FlowerElfredaPower"].UpgradeValueBy(1);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}

// | 楔之刻印 | WedgeBrand | 技能 | 2 | 若目标带有[绽放]，对其施加8/12层[花之楔]。消耗。 |

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
public class WedgeBrand : ModCardTemplate
{
    public WedgeBrand() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<BloomPower>(), HoverTipFactory.FromPower<FlowerWedgePower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FlowerWedgePower>(8)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is null || !cardPlay.Target.HasPower<BloomPower>()) return;
        await PowerCmd.Apply<FlowerWedgePower>(choiceContext, cardPlay.Target, DynamicVars["FlowerWedgePower"].IntValue, Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FlowerWedgePower"].UpgradeValueBy(4);
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );
}

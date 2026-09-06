// | 双重绽放 | DoubleBloom | 技能 | 1 | 若目标带有[绽放]，则对其施加1/2层[绽放]。消耗。 |

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
public class DoubleBloom : ModCardTemplate
{
    public DoubleBloom() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<BloomPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BloomPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target!.HasPower<BloomPower>())
        {
            await PowerCmd.Apply<BloomPower>(choiceContext, Owner.Creature, DynamicVars["BloomPower"].IntValue, Owner.Creature, cardPlay.Card);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BloomPower"].UpgradeValueBy(1);
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
// | 缠绕藤蔓 | EntanglingVines | 技能 | 1 | 施加1/2层[花之楔]。目标失去1/2点力量。 |

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
public class EntanglingVines : ModCardTemplate
{
    public EntanglingVines() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<FlowerWedgePower>(), HoverTipFactory.FromPower<StrengthPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FlowerWedgePower>(1), new PowerVar<StrengthPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is null) return;
        await PowerCmd.Apply<FlowerWedgePower>(choiceContext, cardPlay.Target, DynamicVars["FlowerWedgePower"].IntValue, Owner.Creature, cardPlay.Card);
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, -DynamicVars["StrengthPower"].IntValue, Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FlowerWedgePower"].UpgradeValueBy(1);
        DynamicVars["StrengthPower"].UpgradeValueBy(1);
    }
}

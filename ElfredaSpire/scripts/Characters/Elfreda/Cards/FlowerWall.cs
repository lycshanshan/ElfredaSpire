// | 花之壁 | FlowerWall | 技能 | 1 | 获得6/8点格挡。目标每带有1层[花]，额外获得3/4点格挡。（最多计入3层） |

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using ElfredaSpire.GeneralPowers;

namespace ElfredaSpire.Characters.Elfreda.Cards;

[RegisterCard(typeof(ElfredaCardPool))]
public class FlowerWall : ModCardTemplate
{
    public FlowerWall() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, true)
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

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        // new BlockVar(6, BlockProps.card), // wasted
        new IntVar("ExtraBlock", 3),
        ModCardVars.ComputedBlock("BlockValue", 6, (card, target) => card!.DynamicVars["BlockValue"].BaseValue + ResolveExtraBlock(card, target)),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.EvaluateValueOrDefault("BlockValue", target: Owner.Creature), BlockProps.card, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BlockValue"].UpgradeValueBy(2);
        DynamicVars["ExtraBlock"].UpgradeValueBy(1);
    }

    private static int ResolveExtraBlock(CardModel card, Creature? target)
    {
        int count = Math.Min(target?.GetPowerAmount<FlowerElfredaPower>() ?? 0, 3);
        return count * card.DynamicVars["ExtraBlock"].IntValue;
    }
}

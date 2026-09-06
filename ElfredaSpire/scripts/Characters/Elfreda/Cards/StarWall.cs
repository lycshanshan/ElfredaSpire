// | 星之壁 | StarWall | 技能 | 1 | 获得4点格挡。每有1层[星]，额外获得2/3点格挡。 |

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using ElfredaSpire.GeneralPowers;

namespace ElfredaSpire.Characters.Elfreda.Cards;

[RegisterCard(typeof(ElfredaCardPool))]
public class StarWall : ModCardTemplate
{
    public StarWall() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<StarElfredaPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("ExtraBlock", 2),
        ModCardVars.ComputedBlock("BlockValue", 4, card =>
            card!.DynamicVars["BlockValue"].BaseValue
            + (card.Owner.Creature?.GetPowerAmount<StarElfredaPower>() ?? 0)
            * card.DynamicVars["ExtraBlock"].IntValue),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.EvaluateValueOrDefault("BlockValue"), BlockProps.card, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ExtraBlock"].UpgradeValueBy(1);
    }
}

// | 花之壁 | FlowerWall | 技能 | 1 | 获得6/8点格挡。每有一个带有[花]的敌人，额外获得3/4点格挡。 |

using MegaCrit.Sts2.Core.Entities.Cards;
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
    public FlowerWall() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true)
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
        ModCardVars.ComputedBlock("BlockValue", 6, card => card!.DynamicVars["BlockValue"].BaseValue + ResolveExtraBlock(card)),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.EvaluateValueOrDefault("BlockValue"), BlockProps.card, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BlockValue"].UpgradeValueBy(2);
        DynamicVars["ExtraBlock"].UpgradeValueBy(1);
    }

    private static int ResolveExtraBlock(CardModel card)
    {
        int enemiesWithFlower = card.CombatState?.HittableEnemies.Count(
            enemy => enemy.GetPower<FlowerElfredaPower>() is not null) ?? 0;

        return enemiesWithFlower * card.DynamicVars["ExtraBlock"].IntValue;
    }
}

// | 星光屏障 | StarlightBarrier | 技能 | 1 | 获得7/9点格挡。消耗至多2层[星]，每消耗1层，获得3/5点格挡。 |

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using ElfredaSpire.GeneralPowers;

namespace ElfredaSpire.Characters.Elfreda.Cards;

[RegisterCard(typeof(ElfredaCardPool))]
public class StarlightBarrier : ModCardTemplate
{
    public StarlightBarrier() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
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
        new BlockVar(7, BlockProps.card),
        new IntVar("MaxStarConsume", 2),
        new BlockVar("ExtraBlock", 2, BlockProps.card)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        StarElfredaPower? star = Owner.Creature.GetPower<StarElfredaPower>();
        int amountConsumed = 0;
        if (Owner.Creature.HasPower<GlitterPower>())
        {
            amountConsumed = DynamicVars["MaxStarConsume"].IntValue;
        }
        else if (star is not null)
        {
            int previousAmount = star.Amount;
            int amountToConsume = Math.Min(previousAmount, DynamicVars["MaxStarConsume"].IntValue);
            int newAmount = await PowerCmd.ModifyAmount(
                choiceContext,
                star,
                -amountToConsume,
                Owner.Creature,
                cardPlay.Card);
            amountConsumed = previousAmount - Math.Max(0, newAmount);
        }

        for (int i = 0; i < amountConsumed; i++)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
        DynamicVars["ExtraBlock"].UpgradeValueBy(2);
    }
}

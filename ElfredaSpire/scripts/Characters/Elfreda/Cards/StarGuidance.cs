// | 星之指引 | StarGuidance | 技能 | 1 | 抽2张牌。消耗2层[星]，额外抽1/2张牌。 |

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
public class StarGuidance : ModCardTemplate
{
    public StarGuidance() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true)
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
        new CardsVar(2),
        new PowerVar<StarElfredaPower>(2),
        new IntVar("ExtraCards", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);

        if (!(Owner.Creature.GetPower<StarElfredaPower>()?.Amount >= DynamicVars["StarElfredaPower"].IntValue) && !Owner.Creature.HasPower<GlitterPower>())
        {
            return;
        }

        await PowerCmd.Apply<StarElfredaPower>(choiceContext, Owner.Creature, -DynamicVars["StarElfredaPower"].IntValue, Owner.Creature, cardPlay.Card);
        await CardPileCmd.Draw(choiceContext, DynamicVars["ExtraCards"].IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ExtraCards"].UpgradeValueBy(1);
    }
}

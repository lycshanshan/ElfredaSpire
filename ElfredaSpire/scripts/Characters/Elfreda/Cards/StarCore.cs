// | 星核 | StarCore | 能力 | 2 | 在你的回合开始时，你每有12/10层[星]，获得1点能量。 |

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
public class StarCore : ModCardTemplate
{
    public StarCore() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [HoverTipFactory.FromPower<StarElfredaPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<StarCorePower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (IsUpgraded)
        {
            await PowerCmd.Apply<StarCoreUpgradedPower>(
                choiceContext,
                Owner.Creature,
                DynamicVars["StarCorePower"].IntValue,
                Owner.Creature,
                cardPlay.Card);
        }
        else
        {
            await PowerCmd.Apply<StarCorePower>(
                choiceContext,
                Owner.Creature,
                DynamicVars["StarCorePower"].IntValue,
                Owner.Creature,
                cardPlay.Card);
        }
        
    }

    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );
}

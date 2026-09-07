// | 花之核 | FlowerCore | 能力 | 2 | 施加[花之楔]时，额外施加3层。[花之楔]不再增加受到的伤害。-/[花之楔]在回合结束时层数不再减少。 |

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
public class FlowerCore : ModCardTemplate
{
    public FlowerCore() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [HoverTipFactory.FromPower<FlowerWedgePower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<FlowerCorePower>(3)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int amount = DynamicVars["FlowerCorePower"].IntValue;

        if (IsUpgraded)
        {
            await PowerCmd.Apply<FlowerCoreUpgradedPower>(
                choiceContext,
                Owner.Creature,
                amount,
                Owner.Creature,
                cardPlay.Card);
        }
        else
        {
            await PowerCmd.Apply<FlowerCorePower>(
                choiceContext,
                Owner.Creature,
                amount,
                Owner.Creature,
                cardPlay.Card);
        }
    }

    protected override void OnUpgrade()
    {
    }

    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );
}

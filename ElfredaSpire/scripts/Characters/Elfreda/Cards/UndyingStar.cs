// | 不灭之星 | UndyingStar | 能力 | 1 | 每当你消耗不少于3/2层[星]时，获得1层[星]。 |

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
public class UndyingStar : ModCardTemplate
{
    public UndyingStar() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [HoverTipFactory.FromPower<StarElfredaPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<UndyingStarPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int amount = DynamicVars["UndyingStarPower"].IntValue;

        if (IsUpgraded)
        {
            await PowerCmd.Apply<UndyingStarUpgradedPower>(
                choiceContext,
                Owner.Creature,
                amount,
                Owner.Creature,
                cardPlay.Card);
        }
        else
        {
            await PowerCmd.Apply<UndyingStarPower>(
                choiceContext,
                Owner.Creature,
                amount,
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

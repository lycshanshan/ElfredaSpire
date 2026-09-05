// | 永恒之花 | EternalFlower | 能力 | 2 | 每当你施加[花]时，额外施加1/2层[花之楔]。 |

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
public class EternalFlower : ModCardTemplate
{
    public EternalFlower() : base(2, CardType.Power, CardRarity.Ancient, TargetType.Self, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<FlowerElfredaPower>(),
        HoverTipFactory.FromPower<FlowerWedgePower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<EternalFlowerPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<EternalFlowerPower>(choiceContext, Owner.Creature, DynamicVars["EternalFlowerPower"].IntValue, Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["EternalFlowerPower"].UpgradeValueBy(1);
    }
}
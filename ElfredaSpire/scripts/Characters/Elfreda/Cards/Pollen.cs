// | 花粉 | Pollen | 技能 | 0 | 施加1/2层[花]。获得4/6点格挡。 |

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
public class Pollen : ModCardTemplate
{
    public Pollen() : base(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, true)
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

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FlowerElfredaPower>(1), new BlockVar(4, BlockProps.card)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FlowerElfredaPower>(choiceContext, cardPlay.Target!, DynamicVars["FlowerElfredaPower"].IntValue, Owner.Creature, cardPlay.Card);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FlowerElfredaPower"].UpgradeValueBy(1);
        DynamicVars.Block.UpgradeValueBy(2);
    }
}
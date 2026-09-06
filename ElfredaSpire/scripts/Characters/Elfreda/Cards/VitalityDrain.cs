// | 生机吸收 | VitalityDrain | 技能 | 1/0 | 移除目标的所有[花]并恢复等量生命。消耗。 |

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Commands;
using ElfredaSpire.GeneralPowers;

namespace ElfredaSpire.Characters.Elfreda.Cards;

[RegisterCard(typeof(ElfredaCardPool))]
public class VitalityDrain : ModCardTemplate
{
    public VitalityDrain() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, true)
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

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is null) return;

        FlowerElfredaPower? flower = cardPlay.Target.GetPower<FlowerElfredaPower>();
        if (flower is null)
        {
            return;
        }

        int flowerAmount = flower.Amount;
        await PowerCmd.Remove(flower);
        await CreatureCmd.Heal(Owner.Creature, flowerAmount);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}

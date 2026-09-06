// | 花之刺 | FlowerSting | 攻击 | 1 | 造成4/5点伤害两次。若目标带有[花]，施加1/2层易伤。 |

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using ElfredaSpire.GeneralPowers;

namespace ElfredaSpire.Characters.Elfreda.Cards;

[RegisterCard(typeof(ElfredaCardPool))]
public class FlowerSting : ModCardTemplate
{
    public FlowerSting() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<FlowerElfredaPower>(), HoverTipFactory.FromPower<VulnerablePower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4, ValueProp.Move), new PowerVar<VulnerablePower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is null) { return; }
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(2).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        if (cardPlay.Target.HasPower<FlowerElfredaPower>())
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, DynamicVars["VulnerablePower"].IntValue, Owner.Creature, cardPlay.Card);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1);
        DynamicVars["VulnerablePower"].UpgradeValueBy(1);
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );
}

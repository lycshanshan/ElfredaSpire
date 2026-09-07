// | 标记引爆-楔 | WedgeDetonation | 攻击 | 2 | 目标每有2/1层[花之楔]，造成8/6点伤害一次。移除目标的所有[花之楔]。（造成伤害{HitCount:diff()}次。） |

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using ElfredaSpire.GeneralPowers;

namespace ElfredaSpire.Characters.Elfreda.Cards;

[RegisterCard(typeof(ElfredaCardPool))]
public class WedgeDetonation : ModCardTemplate
{
    public WedgeDetonation() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<FlowerWedgePower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(8, ValueProp.Move),
        new IntVar("FlowerWedgeRequired", 2),
        ModCardVars.Computed("HitCount", 0, (card, target) => (target?.GetPowerAmount<FlowerWedgePower>() ?? 0) / DynamicVars["FlowerWedgeRequired"].IntValue),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount((int)DynamicVars.EvaluateValueOrDefault("HitCount", target: cardPlay.Target))
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
        
        FlowerWedgePower? flowerWedge = cardPlay.Target.GetPower<FlowerWedgePower>();
        if (flowerWedge is null) { return; }
        await PowerCmd.Remove(flowerWedge);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(-2);
        DynamicVars["FlowerWedgeRequired"].UpgradeValueBy(-1);
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );
}
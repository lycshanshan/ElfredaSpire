// | 楔击 | WedgeStrike | 攻击 | 1 | 造成6点伤害。目标每有一层[花之楔]，额外造成2/3点伤害。 |

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
public class WedgeStrike : ModCardTemplate
{
    public WedgeStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<FlowerWedgePower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(6, ValueProp.Move), // now wasted
        new IntVar("ExtraDamage", 2),
        ModCardVars.ComputedDamage("DamageValue", 6, (card, target) => card!.DynamicVars["DamageValue"].BaseValue + (target?.GetPowerAmount<FlowerWedgePower>() ?? 0) * DynamicVars["ExtraDamage"].IntValue),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is null) return;
        await DamageCmd.Attack(DynamicVars.EvaluateValueOrDefault("DamageValue", target: cardPlay.Target)).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        // await DamageCmd.Attack(DynamicVars.GetComputedValue("DamageValue")).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ExtraDamage"].UpgradeValueBy(1);
    }
}

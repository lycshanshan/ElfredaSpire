// | 星云波动 | NebulaWave | 攻击 | 1 | 造成10/12点伤害。消耗1层[星]，对所有敌人造成6/8点伤害。 |

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
public class NebulaWave : ModCardTemplate
{
    public NebulaWave() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
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
        new DamageVar(10, ValueProp.Move),
        new PowerVar<StarElfredaPower>(1),
        new DamageVar("StarDamage", 6, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);

        if (!(Owner.Creature.GetPower<StarElfredaPower>()?.Amount >= DynamicVars["StarElfredaPower"].IntValue) && !Owner.Creature.HasPower<GlitterPower>())
        {
            return;
        }
        await PowerCmd.Apply<StarElfredaPower>(choiceContext, Owner.Creature, -DynamicVars["StarElfredaPower"].IntValue, Owner.Creature, cardPlay.Card);
        await DamageCmd.Attack(DynamicVars["StarDamage"].BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState!)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars["StarDamage"].UpgradeValueBy(2);
    }
}

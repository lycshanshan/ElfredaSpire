// | 花楔深埋 | DeepWedgeImplant | 攻击 | 1 | 造成5/6点伤害两次。造成未被格挡的伤害时，施加1/2层[花之楔]。 |

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using ElfredaSpire.GeneralPowers;
using STS2RitsuLib.Combat.AttackHits;

namespace ElfredaSpire.Characters.Elfreda.Cards;

[RegisterCard(typeof(ElfredaCardPool))]
public class DeepWedgeImplant : ModCardTemplate, IAttackHitHookListener
{
    public DeepWedgeImplant() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<FlowerWedgePower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5, ValueProp.Move), new PowerVar<FlowerWedgePower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is null) { return; }
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(2).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
    }

    public async Task AfterAttackHit(AttackHitContext context)
    {
        if (context.CardSource != this || context.Dealer != Owner.Creature) { return; }

        foreach (var result in context.Results)
        {
            if (result.UnblockedDamage > 0)
            {
                await PowerCmd.Apply<FlowerWedgePower>(context.ChoiceContext, result.Receiver, DynamicVars["FlowerWedgePower"].IntValue, Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1);
        DynamicVars["FlowerWedgePower"].UpgradeValueBy(1);
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );
}

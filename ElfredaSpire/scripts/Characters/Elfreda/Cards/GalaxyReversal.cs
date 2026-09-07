// | 星河倒转 | GalaxyReversal | 攻击 | X | 随机对敌人造成8/10点伤害X/X+1次。消耗至多3/4层[星]，并获得等量的能量。 |

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
public class GalaxyReversal : ModCardTemplate
{
    protected override bool HasEnergyCostX => true;

    public GalaxyReversal() : base(-1, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy, true)
    {
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<StarElfredaPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8, ValueProp.Move),
        new IntVar("ExtraHits", 0),
        new IntVar("MaxStarConsume", 3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int hitCount = ResolveEnergyXValue() + DynamicVars["ExtraHits"].IntValue;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(hitCount)
            .FromCard(this)
            .TargetingRandomOpponents(CombatState!)
            .Execute(choiceContext);

        StarElfredaPower? star = Owner.Creature.GetPower<StarElfredaPower>();
        int amountConsumed = 0;
        if (Owner.Creature.HasPower<GlitterPower>())
        {
            amountConsumed = DynamicVars["MaxStarConsume"].IntValue;
        }
        else if (star is not null)
        {
            int previousAmount = star.Amount;
            int amountToConsume = Math.Min(previousAmount, DynamicVars["MaxStarConsume"].IntValue);
            int newAmount = await PowerCmd.ModifyAmount(
                choiceContext,
                star,
                -amountToConsume,
                Owner.Creature,
                cardPlay.Card);
            amountConsumed = previousAmount - Math.Max(0, newAmount);
        }

        if (amountConsumed > 0)
        {
            await PlayerCmd.GainEnergy(amountConsumed, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars["ExtraHits"].UpgradeValueBy(1);
        DynamicVars["MaxStarConsume"].UpgradeValueBy(1);
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );
}

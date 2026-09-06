/*
- 造成的伤害减少[层数+5]%。
- 1~5层：失去1点力量。
- 6~7层：失去2点力量。
- 8~10层：失去2点力量，每次攻击时获得1层[花]。
- 11~20层：失去3点力量，每次攻击时获得1层[花]。
- 21~30层：失去3点力量，每次攻击时获得2层[花]。
- 达到20层时，立即移除本效果，并获得3层[绽放]。
- 回合结束时，减少一层。
*/

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace ElfredaSpire.GeneralPowers;

[RegisterPower]
public class FlowerElfredaPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<BloomPower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("DamageReduction", 5)];

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack())
        {
            return 1m;
        }

        return 1m - (Amount + 5) / 100m;
    }

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power != this)
        {
            return;
        }
        DynamicVars["DamageReduction"].BaseValue = Amount + 5;

        int previousAmount = Amount - (int)amount;
        int previousStrengthLoss = GetStrengthLoss(previousAmount);
        int currentStrengthLoss = GetStrengthLoss(Amount);
        int strengthChange = previousStrengthLoss - currentStrengthLoss;

        if (strengthChange != 0)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, strengthChange, null, null);
        }

        if (Amount >= 30)
        {
            Flash();
            await PowerCmd.Remove(this);
            // await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, GetStrengthLoss(Amount), null, null);
            await PowerCmd.Apply<BloomPower>(choiceContext, Owner, 3, null, null);
        }
    }

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (Amount < 8 ||
            command.Attacker != Owner ||
            command.TargetSide == Owner.Side ||
            !command.DamageProps.IsPoweredAttack())
        {
            return;
        }

        int flowerGained = Amount >= 21 ? 2 : 1;
        Flash();
        await PowerCmd.ModifyAmount(choiceContext, this, flowerGained, Owner, null);
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            Flash();
            await PowerCmd.Decrement(this);
        }
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        int strengthLoss = GetStrengthLoss(Amount);
        if (strengthLoss > 0)
        {
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), oldOwner, strengthLoss, oldOwner, null);
        }
    }

    private static int GetStrengthLoss(int amount)
    {
        return amount switch
        {
            >= 11 => 3,
            >= 6 => 2,
            >= 1 => 1,
            _ => 0
        };
    }

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );
}

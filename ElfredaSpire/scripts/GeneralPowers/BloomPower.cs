// 失去5点力量。造成的伤害减少50%，受到的伤害增加50%。不能被施加[花]，改为每被施加一层，受到10点伤害并失去2点临时力量。回合结束时，减少一层。

using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace ElfredaSpire.GeneralPowers;

[RegisterPower]
public class BloomPower : ModPowerTemplate
{
    private int _pendingFlowerGained;

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner, -5, applier, cardSource);
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), oldOwner, 5, oldOwner, null);
    }

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (!props.IsPoweredAttack())
        {
            return 1m;
        }

        decimal multiplier = 1m;

        if (dealer == Owner)
        {
            multiplier *= 0.5m;
        }

        if (target == Owner)
        {
            multiplier *= 1.5m;
        }

        return multiplier;
    }

    public override bool TryModifyPowerAmountReceived(
        PowerModel canonicalPower,
        Creature target,
        decimal amount,
        Creature? applier,
        out decimal modifiedAmount)
    {
        if (target == Owner && canonicalPower is FlowerElfredaPower && amount > 0m)
        {
            _pendingFlowerGained += (int)amount;
            modifiedAmount = 0m;
            return true;
        }

        modifiedAmount = amount;
        return false;
    }

    public override async Task AfterModifyingPowerAmountReceived(PowerModel power)
    {
        if (power is not FlowerElfredaPower || _pendingFlowerGained <= 0)
        {
            return;
        }

        int flowerGained = _pendingFlowerGained;
        _pendingFlowerGained = 0;
        Flash();

        var choiceContext = new ThrowingPlayerChoiceContext();
        for (int i = 0; i < flowerGained; i++)
        {
            await CreatureCmd.Damage(choiceContext, Owner, 10, ValueProp.Unpowered, Owner, null);
            await PowerCmd.Apply<TempStrengthPower>(choiceContext, Owner, -2, Owner, null);
        }
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

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );
}

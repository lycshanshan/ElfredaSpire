using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;

namespace ElfredaSpire.GeneralPowers;

[RegisterPower]
public class FlowerCoreUpgradedPower : FlowerCorePower
{
    private bool _isSideTurnEnding;

    public override Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        _isSideTurnEnding = true;
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnEndLate(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        _isSideTurnEnding = false;
        return Task.CompletedTask;
    }

    public override bool TryModifyPowerAmountReceived(
        PowerModel canonicalPower,
        Creature target,
        decimal amount,
        Creature? applier,
        out decimal modifiedAmount)
    {
        if (_isSideTurnEnding && canonicalPower is FlowerWedgePower && amount < 0m)
        {
            modifiedAmount = 0m;
            return true;
        }

        modifiedAmount = amount;
        return false;
    }
}

using STS2RitsuLib.Interop.AutoRegistration;

namespace ElfredaSpire.GeneralPowers;

[RegisterPower]
public class UndyingStarUpgradedPower : UndyingStarPower
{
    protected override int StarConsumptionThreshold => 2;
}

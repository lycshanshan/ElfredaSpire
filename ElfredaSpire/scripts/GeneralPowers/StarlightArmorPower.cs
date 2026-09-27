using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace ElfredaSpire.GeneralPowers;

[RegisterPower]
public class StarlightArmorPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforePowerAmountChanged(
        PowerModel power,
        decimal amount,
        Creature target,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power is not StarElfredaPower || target != Owner || amount >= 0m ||
            !Owner.HasPower<GlitterPower>())
        {
            return;
        }

        // int previousAmount = power.Amount - (int)amount;
        // int amountConsumed = Math.Max(0, previousAmount) - Math.Max(0, power.Amount);
        // if (amountConsumed <= 0)
        // {
        //     return;
        // }

        await Trigger(amount);
    }

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power is not StarElfredaPower || power.Owner != Owner || amount >= 0m)
        {
            return;
        }

        await Trigger(amount);
    }

    private async Task Trigger(decimal amount)
    {
        Flash();
        await CreatureCmd.GainBlock(Owner, Amount * (-(int)amount), ValueProp.Unpowered, null);
    }

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );
}

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace ElfredaSpire.GeneralPowers;

[RegisterPower]
public class UndyingStarPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected virtual int StarConsumptionThreshold => 3;

    public override async Task BeforePowerAmountChanged(
        PowerModel power,
        decimal amount,
        Creature target,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power is not StarElfredaPower
            || target != Owner
            || amount > -StarConsumptionThreshold
            || !Owner.HasPower<GlitterPower>())
        {
            return;
        }

        await Trigger(new ThrowingPlayerChoiceContext(), cardSource);
    }

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power is not StarElfredaPower
            || power.Owner != Owner
            || amount > -StarConsumptionThreshold)
        {
            return;
        }

        await Trigger(choiceContext, cardSource);
    }

    private async Task Trigger(PlayerChoiceContext choiceContext, CardModel? cardSource)
    {
        Flash();
        await PowerCmd.Apply<StarElfredaPower>(choiceContext, Owner, Amount, Owner, cardSource);
    }

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );
}

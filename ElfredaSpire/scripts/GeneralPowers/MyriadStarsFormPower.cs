using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace ElfredaSpire.GeneralPowers;

[RegisterPower]
public class MyriadStarsFormPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public int Amount2;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        Amount2 = Amount * 2;
    }

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power is MyriadStarsFormPower && power.Owner == Owner && amount != 0m || Owner.Player is not null)
        {
            Amount2 += (int)amount * 2;
            return;
        }

        if (power is not StarElfredaPower || power.Owner != Owner || amount == 0m || Owner.Player is null)
        {
            return;
        }

        Flash();
        await PlayerCmd.GainEnergy(Amount, Owner.Player);
        await CardPileCmd.Draw(choiceContext, Amount * 2, Owner.Player);
    }

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );
}

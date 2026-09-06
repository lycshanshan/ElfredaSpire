/*
- 造成的伤害增加[层数]%。
- 5~7层：获得1层敏捷。
- 8~10层：获得1层力量和敏捷。
- 11~17层：获得1层力量，2层敏捷，受到的伤害减少10%。
- 18~25层：获得2层力量和敏捷，受到的伤害减少20%。
- 26~50层：获得2层力量和敏捷，受到的伤害减少25%，每回合额外获得1点能量。
- 达到50层时，立即移除本效果，并获得[闪耀]。
*/

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
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
public class StarElfredaPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<GlitterPower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

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
            multiplier *= 1m + Amount / 100m;
        }

        if (target == Owner)
        {
            multiplier *= 1m - GetDamageReduction();
        }

        return multiplier;
    }

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power != this || amount <= 0m)
        {
            return;
        }

        int previousAmount = Amount - (int)amount;
        if (previousAmount < 50 && Amount >= 50)
        {
            Flash();
            await PowerCmd.Remove(this);
            // await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, -GetStrengthBonus(Amount), Owner, null);
            // await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, -GetDexterityBonus(Amount), Owner, null);
            await PowerCmd.Apply<GlitterPower>(choiceContext, Owner, 1, Owner, null);
            return;
        }

        int strengthGained = GetStrengthBonus(Amount) - GetStrengthBonus(previousAmount);
        int dexterityGained = GetDexterityBonus(Amount) - GetDexterityBonus(previousAmount);

        if (strengthGained > 0)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, strengthGained, Owner, null);
        }

        if (dexterityGained > 0)
        {
            await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, dexterityGained, Owner, null);
        }
    }

    public override async Task AfterEnergyReset(Player player)
    {
        if (player == Owner.Player && Amount >= 26)
        {
            Flash();
            await PlayerCmd.GainEnergy(1m, player);
        }
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        int strengthBonus = GetStrengthBonus(Amount);
        int dexterityBonus = GetDexterityBonus(Amount);
        if (strengthBonus > 0)
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), oldOwner, -strengthBonus, oldOwner, null);
        if (dexterityBonus > 0)
            await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), oldOwner, -dexterityBonus, oldOwner, null);
    }

    private decimal GetDamageReduction()
    {
        return Amount switch
        {
            >= 26 => 0.25m,
            >= 18 => 0.20m,
            >= 11 => 0.10m,
            _ => 0m
        };
    }

    private static int GetStrengthBonus(int amount)
    {
        return amount switch
        {
            >= 18 => 2,
            >= 8 => 1,
            _ => 0
        };
    }

    private static int GetDexterityBonus(int amount)
    {
        return amount switch
        {
            >= 11 => 2,
            >= 5 => 1,
            _ => 0
        };
    }

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );
}

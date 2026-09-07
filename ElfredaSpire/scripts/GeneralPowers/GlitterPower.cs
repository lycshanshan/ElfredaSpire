// 获得5点力量和敏捷。造成的伤害增加50%，受到的伤害减少25%。每回合额外获得2点能量。所有消耗[星]的牌不再消耗[星]且视为最大消耗量。不能获得[星]，改为每获得1层，对全体敌方单位造成10点伤害并施加2层[花]。

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace ElfredaSpire.GeneralPowers;

[RegisterPower]
public class GlitterPower : ModPowerTemplate
{
    private int _pendingStarGained;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(2)];

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var choiceContext = new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, 5, Owner, cardSource);
        await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, 5, Owner, cardSource);
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        var choiceContext = new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<StrengthPower>(choiceContext, oldOwner, -5, oldOwner, null);
        await PowerCmd.Apply<DexterityPower>(choiceContext, oldOwner, -5, oldOwner, null);
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
            multiplier *= 1.5m;
        }

        if (target == Owner)
        {
            multiplier *= 0.75m;
        }

        return multiplier;
    }

    public override async Task AfterEnergyReset(Player player)
    {
        if (player == Owner.Player)
        {
            Flash();
            await PlayerCmd.GainEnergy(2m, player);
        }
    }

    public override bool TryModifyPowerAmountReceived(
        PowerModel canonicalPower,
        Creature target,
        decimal amount,
        Creature? applier,
        out decimal modifiedAmount)
    {
        if (target == Owner && canonicalPower is StarElfredaPower)// && amount > 0m
        {
            _pendingStarGained += (int)amount;
            modifiedAmount = 0m;
            return true;
        }

        modifiedAmount = amount;
        return false;
    }

    public override async Task AfterModifyingPowerAmountReceived(PowerModel power)
    {
        if (power is not StarElfredaPower || _pendingStarGained <= 0)
        {
            return;
        }

        int starGained = _pendingStarGained;
        _pendingStarGained = 0;
        Flash();

        var combatState = Owner.CombatState;
        if (combatState is null)
        {
            return;
        }

        var choiceContext = new ThrowingPlayerChoiceContext();
        for (int i = 0; i < starGained; i++)
        {
            Creature[] enemies = combatState.HittableEnemies.ToArray();
            await CreatureCmd.Damage(choiceContext, enemies, 10, ValueProp.Unpowered, Owner, null);
            await PowerCmd.Apply<FlowerElfredaPower>(choiceContext, enemies, 2, Owner, null);
        }
    }

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );
}

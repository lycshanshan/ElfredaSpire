// 获得3点力量和敏捷。每回合额外获得2点能量。所有消耗[星]的牌不再消耗[星]且视为最大消耗量。不能获得[星]，改为每获得1层，对随机敌方单位造成15点伤害并施加1层[花之楔]。

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
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, 3, Owner, cardSource);
        await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, 3, Owner, cardSource);
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        var choiceContext = new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<StrengthPower>(choiceContext, oldOwner, -3, oldOwner, null);
        await PowerCmd.Apply<DexterityPower>(choiceContext, oldOwner, -3, oldOwner, null);
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
            var enemies = CombatState?.HittableEnemies.ToList();
            if (enemies is null || enemies.Count == 0) { return; }
            Creature? target = Owner.Player?.RunState.Rng.CombatTargets.NextItem(enemies);
            if (target is null) { return; }
            await CreatureCmd.Damage(choiceContext, target, 15, ValueProp.Unpowered, Owner, null);
            await PowerCmd.Apply<FlowerWedgePower>(choiceContext, target, 1, Owner, null);
        }
    }

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );
}

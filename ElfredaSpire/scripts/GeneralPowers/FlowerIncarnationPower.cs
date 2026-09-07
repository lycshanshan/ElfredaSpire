using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace ElfredaSpire.GeneralPowers;

[RegisterPower]
public class FlowerIncarnationPower : ModPowerTemplate
{
    private bool _isApplyingTriggeredFlower;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (_isApplyingTriggeredFlower ||
            power is not FlowerElfredaPower ||
            amount <= 0m ||
            applier != Owner)
        {
            return;
        }

        var enemies = CombatState?.HittableEnemies.ToList();
        if (enemies is null || enemies.Count == 0)
        {
            return;
        }

        var player = Owner.Player;
        if (player is null)
        {
            return;
        }

        Creature? target = player.RunState.Rng.CombatTargets.NextItem(enemies);
        if (target is null)
        {
            return;
        }

        Flash();

        _isApplyingTriggeredFlower = true;
        try
        {
            await PowerCmd.Apply<FlowerElfredaPower>(choiceContext, target, Amount, Owner, cardSource);
        }
        finally
        {
            _isApplyingTriggeredFlower = false;
        }
    }

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );
}

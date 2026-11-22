using MegaCrit.Sts2.Core.Combat;
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
public class StarlightRecyclePower : ModPowerTemplate
{
    private sealed class Data
    {
        public readonly Queue<(PlayerChoiceContext ChoiceContext, CardModel? CardSource)> PendingRecycles = new();
        public bool IsProcessing;
        public bool DealtEffectiveDamage;
    }

    private const int MaxConsecutiveRecyclesWithoutDamage = 10;
    private const int MaxRecyclesPerChain = 1000;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData() => new Data();

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power is not FlowerWedgePower || amount <= 0m || applier != Owner)
        {
            return;
        }

        var data = GetInternalData<Data>();
        data.PendingRecycles.Enqueue((choiceContext, cardSource));
        if (data.IsProcessing)
        {
            return;
        }

        data.IsProcessing = true;
        try
        {
            int totalRecycles = 0;
            int consecutiveRecyclesWithoutDamage = 0;

            // 连锁产生的回收依次结算，避免在给予星的过程中递归执行下一次回收。
            while (totalRecycles < MaxRecyclesPerChain
                && consecutiveRecyclesWithoutDamage < MaxConsecutiveRecyclesWithoutDamage
                && !CombatManager.Instance.IsEnding
                && data.PendingRecycles.TryDequeue(out var pending))
            {
                totalRecycles++;
                data.DealtEffectiveDamage = false;
                Flash();
                await PowerCmd.Apply<StarElfredaPower>(pending.ChoiceContext, Owner, Amount, Owner, pending.CardSource);
                consecutiveRecyclesWithoutDamage = data.DealtEffectiveDamage
                    ? 0
                    : consecutiveRecyclesWithoutDamage + 1;
            }
        }
        finally
        {
            data.PendingRecycles.Clear();
            data.IsProcessing = false;
            data.DealtEffectiveDamage = false;
        }
    }

    public override Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        var data = GetInternalData<Data>();
        if (data.IsProcessing
            && dealer == Owner
            && result.Receiver.Side != Owner.Side
            && (result.BlockedDamage > 0
                || (!result.Receiver.HpDisplay.IsInfinite() && result.UnblockedDamage > 0)))
        {
            data.DealtEffectiveDamage = true;
        }

        return Task.CompletedTask;
    }

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );
}

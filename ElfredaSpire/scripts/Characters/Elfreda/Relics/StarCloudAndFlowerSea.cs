// 包含一个[Count]。战斗开始时，获得[Count]+15层[星]，对所有敌人施加[Count]层[花]。战斗中，每次获得[闪耀]或生成[绽放]，升级。

using ElfredaSpire.GeneralPowers;
using ElfredaSpire.Characters.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Patching.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace ElfredaSpire.Characters.Elfreda.Relics;

[RegisterRelic(typeof(ElfredaRelicPool))]
public class StarCloudAndFlowerSea : ModRelicTemplate
{
    private const int StartingCount = 3;

    private int _count = StartingCount;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => true;
    public override int DisplayAmount => Count;

    [SavedProperty]
    public int Count
    {
        get => _count;
        set
        {
            AssertMutable();
            _count = value;
            DynamicVars["Count"].BaseValue = value;
            DynamicVars["StarAmount"].BaseValue = value + 15;
            InvokeDisplayAmountChanged();
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Count", StartingCount),
        new DynamicVar("StarAmount", StartingCount + 15)
    ];

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        IconOutlinePath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png"
    );

    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (player != Owner || options.Any(option => option is ElfredaRestSiteOption))
        {
            return false;
        }

        options.Add(new ElfredaRestSiteOption(player));
        return true;
    }

    public override async Task BeforeCombatStart()
    {
        int countAtCombatStart = Count;
        var choiceContext = new ThrowingPlayerChoiceContext();

        await PowerCmd.Apply<StarElfredaPower>(choiceContext, Owner.Creature, countAtCombatStart + 15, Owner.Creature, null);

        Creature[] enemies = Owner.Creature.CombatState?.HittableEnemies.ToArray() ?? [];
        await PowerCmd.Apply<FlowerElfredaPower>(choiceContext, enemies, countAtCombatStart, Owner.Creature, null);
    }

    public override Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (amount <= 0m)
        {
            return Task.CompletedTask;
        }

        bool gainedGlitter = power is GlitterPower && power.Owner == Owner.Creature;
        bool enemyGainedBloom = power is BloomPower && power.Owner.Side != Owner.Creature.Side;
        if (gainedGlitter || enemyGainedBloom)
        {
            Count++;
            Flash();
        }

        return Task.CompletedTask;
    }
}

public sealed class PreserveStarAndFlowerCountOnReplacementPatch : IPatchMethod
{
    public static string PatchId => "elfreda_preserve_star_and_flower_count_on_replacement";
    public static string Description => "Preserve StarAndFlower count when Touch of Orobas replaces it";
    public static bool IsCritical => false;

    public static ModPatchTarget[] GetTargets() =>
    [
        new(typeof(RelicCmd), nameof(RelicCmd.Replace), [typeof(RelicModel), typeof(RelicModel)])
    ];

    public static void Prefix(RelicModel original, RelicModel replace)
    {
        if (original is StarAndFlower starAndFlower && replace is StarCloudAndFlowerSea starCloudAndFlowerSea)
        {
            starCloudAndFlowerSea.Count = starAndFlower.Count;
        }
    }
}

// 包含一个[Count]。战斗开始时，获得[Count]层[星]。每场战斗[星]的层数第一次达到[Count]*2时，升级。

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
using STS2RitsuLib.Scaffolding.Content;

namespace ElfredaSpire.Characters.Elfreda.Relics;

[RegisterRelic(typeof(ElfredaRelicPool))]
[RegisterCharacterStarterRelic(typeof(ElfredaCharacter))]
public class StarAndFlower : ModRelicTemplate
{
    private const int StartingCount = 3;

    private int _count = StartingCount;
    private bool _upgradedThisCombat;

    public override RelicRarity Rarity => RelicRarity.Starter;

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
            InvokeDisplayAmountChanged();
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Count", StartingCount)];

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
        _upgradedThisCombat = false;
        await PowerCmd.Apply<StarElfredaPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, Count, Owner.Creature, null);
    }

    public override Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (_upgradedThisCombat ||
            amount <= 0m ||
            power is not StarElfredaPower ||
            power.Owner != Owner.Creature)
        {
            return Task.CompletedTask;
        }

        int previousAmount = power.Amount - (int)amount;
        int threshold = Count * 2;
        if (previousAmount < threshold && power.Amount >= threshold)
        {
            _upgradedThisCombat = true;
            Count++;
            Flash();
        }

        return Task.CompletedTask;
    }
}

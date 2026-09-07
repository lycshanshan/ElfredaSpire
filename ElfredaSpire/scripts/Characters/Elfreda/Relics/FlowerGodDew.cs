// 战斗开始时，对随机1名敌人施加3层[花]。

using ElfredaSpire.GeneralPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace ElfredaSpire.Characters.Elfreda.Relics;

[RegisterRelic(typeof(ElfredaRelicPool))]
public class FlowerGodDew : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<FlowerElfredaPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<FlowerElfredaPower>(3)
    ];

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        IconOutlinePath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png"
    );

    public override async Task BeforeCombatStart()
    {
        List<Creature>? enemies = Owner.Creature.CombatState?.HittableEnemies.ToList();
        if (enemies is null || enemies.Count == 0)
        {
            return;
        }

        Creature? target = Owner.RunState.Rng.CombatTargets.NextItem(enemies);
        if (target is null)
        {
            return;
        }

        Flash();
        await PowerCmd.Apply<FlowerElfredaPower>(
            new ThrowingPlayerChoiceContext(),
            target,
            DynamicVars["FlowerElfredaPower"].IntValue,
            Owner.Creature,
            null);
    }
}

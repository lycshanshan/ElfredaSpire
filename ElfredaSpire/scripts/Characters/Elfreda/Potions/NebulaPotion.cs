using ElfredaSpire.Characters.Elfreda;
using ElfredaSpire.GeneralPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace ElfredaSpire.Script.Characters.Elfreda.Potions;

[RegisterPotion(typeof(ElfredaPotionPool))]
public class NebulaPotion : ModPotionTemplate
{
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override PotionRarity Rarity => PotionRarity.Common;
    public override TargetType TargetType => TargetType.Self;
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<StarElfredaPower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StarElfredaPower>(5)];

    public override PotionAssetProfile AssetProfile => new(
        ImagePath: $"{Entry.ResPath}/images/potions/{GetType().Name}.png",
        OutlinePath: $"{Entry.ResPath}/images/potions/{GetType().Name}.png"
    );

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (target is null) return;
        await PowerCmd.Apply<StarElfredaPower>(choiceContext, target, DynamicVars["StarElfredaPower"].IntValue, Owner.Creature, null);
    }
}

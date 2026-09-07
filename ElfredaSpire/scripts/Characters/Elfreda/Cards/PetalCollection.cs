// | 花瓣收集 | PetalCollection | 攻击 | 2 | 造成10点伤害。本场战斗中每施加过1次[花]，这张牌的伤害+2/3。 |

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Commands;
using ElfredaSpire.GeneralPowers;

namespace ElfredaSpire.Characters.Elfreda.Cards;

[RegisterCard(typeof(ElfredaCardPool))]
public class PetalCollection : ModCardTemplate
{
    public PetalCollection() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }
protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<FlowerElfredaPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("ExtraDamage", 2),
        ModCardVars.ComputedDamage("DamageValue", 10, (card) => card!.DynamicVars["DamageValue"].BaseValue + CalculateExtraDamage())
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is null) return;
        await DamageCmd.Attack(DynamicVars.EvaluateValueOrDefault("DamageValue", target: cardPlay.Target)).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
    }

    private int CalculateExtraDamage()
    {
        int flowersApplied = CombatManager.Instance.History.Entries
            .OfType<PowerReceivedEntry>()
            .Count(entry => entry.Power is FlowerElfredaPower &&
                            entry.Power.Applier == Owner.Creature &&
                            entry.Amount > 0m);
            // .Where(entry => entry.Power is FlowerElfredaPower
            //     && entry.Power.Applier == Owner.Creature
            //     && entry.Amount > 0m)
            // .Sum(entry => (int)entry.Amount);

        return flowersApplied * DynamicVars["ExtraDamage"].IntValue;
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ExtraDamage"].UpgradeValueBy(1);
    }

    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );
}

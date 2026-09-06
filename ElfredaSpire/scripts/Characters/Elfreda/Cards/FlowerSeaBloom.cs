// | 花海绽放 | FlowerSeaBloom | 技能 | 2 | 对所有带有[花]的敌人施加2层易伤。若其带有不低于12/10层[花]，则移除其所有[花]并对其施加2层[绽放]。消耗。 |

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Commands;
using ElfredaSpire.GeneralPowers;

namespace ElfredaSpire.Characters.Elfreda.Cards;

[RegisterCard(typeof(ElfredaCardPool))]
public class FlowerSeaBloom : ModCardTemplate
{
    public FlowerSeaBloom() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<FlowerElfredaPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<BloomPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VulnerablePower>(2),
        new DynamicVar("FlowerRequired", 12),
        new PowerVar<BloomPower>(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState is null)
        {
            return;
        }

        foreach (var enemy in CombatState.HittableEnemies.ToList())
        {
            FlowerElfredaPower? flower = enemy.GetPower<FlowerElfredaPower>();
            if (flower is null)
            {
                continue;
            }

            await PowerCmd.Apply<VulnerablePower>(choiceContext, enemy, DynamicVars["VulnerablePower"].IntValue, Owner.Creature, cardPlay.Card);

            if (flower.Amount >= DynamicVars["FlowerRequired"].IntValue)
            {
                await PowerCmd.Remove(flower);
                await PowerCmd.Apply<BloomPower>(choiceContext, enemy, DynamicVars["BloomPower"].IntValue, Owner.Creature, cardPlay.Card);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FlowerRequired"].UpgradeValueBy(-2);
    }
}

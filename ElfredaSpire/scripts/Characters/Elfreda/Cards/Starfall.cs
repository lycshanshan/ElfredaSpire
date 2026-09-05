// | 星陨 | Starfall | 攻击 | 3 | 造成20/27点伤害。获得5/7层[星]。移除目标的所有[花]并施加等量的[花之楔]。 |

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using ElfredaSpire.GeneralPowers;

namespace ElfredaSpire.Characters.Elfreda.Cards;

[RegisterCard(typeof(ElfredaCardPool))]
public class Starfall : ModCardTemplate
{
    public Starfall() : base(3, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => 
    [
        HoverTipFactory.FromPower<StarElfredaPower>(),
        HoverTipFactory.FromPower<FlowerElfredaPower>(),
        HoverTipFactory.FromPower<FlowerWedgePower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(20, ValueProp.Move), new PowerVar<StarElfredaPower>(5)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        await PowerCmd.Apply<StarElfredaPower>(choiceContext, Owner.Creature, DynamicVars["StarElfredaPower"].IntValue, Owner.Creature, cardPlay.Card);

        FlowerElfredaPower? flower = cardPlay.Target.GetPower<FlowerElfredaPower>();
        if (flower is not null)
        {
            int flowerAmount = flower.Amount;
            await PowerCmd.Remove(flower);
            await PowerCmd.Apply<FlowerWedgePower>(choiceContext, cardPlay.Target, flowerAmount, Owner.Creature, cardPlay.Card);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(7);
        DynamicVars["StarElfredaPower"].UpgradeValueBy(2);
    }
}

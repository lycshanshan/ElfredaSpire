// | 星光流转 | StarlightFlow | 攻击 | 1 | 造成10/14点伤害。消耗至多3/4层[星]，并获得消耗层数两倍的[星]。 |

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
public class StarlightFlow : ModCardTemplate
{
    public StarlightFlow() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<StarElfredaPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10, ValueProp.Move), new IntVar("MaxStarConsume", 3)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target!).Execute(choiceContext);

        StarElfredaPower? star = Owner.Creature.GetPower<StarElfredaPower>();
        int amountConsumed = 0;
        if (Owner.Creature.HasPower<GlitterPower>())
        {
            amountConsumed = DynamicVars["MaxStarConsume"].IntValue;
        }
        else if (star is not null)
        {
            int previousAmount = star.Amount;
            int amountToConsume = Math.Min(previousAmount, DynamicVars["MaxStarConsume"].IntValue);
            int newAmount = await PowerCmd.ModifyAmount(
                choiceContext,
                star,
                -amountToConsume,
                Owner.Creature,
                cardPlay.Card);
            amountConsumed = previousAmount - Math.Max(0, newAmount);
        }

        if (amountConsumed > 0)
        {
            await PowerCmd.Apply<StarElfredaPower>(choiceContext, Owner.Creature, amountConsumed * 2, Owner.Creature, cardPlay.Card);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
        DynamicVars["MaxStarConsume"].UpgradeValueBy(1);
    }
}

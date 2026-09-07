// | 繁星 | MyriadStars | 攻击 | 1 | 消耗至多2/3层[星]。每消耗1层，造成8/10点伤害1次。 |

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
public class MyriadStars : ModCardTemplate
{
    public MyriadStars() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<StarElfredaPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move), new IntVar("MaxStarConsume", 2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is null) { return; }

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
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .WithHitCount(amountConsumed)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars["MaxStarConsume"].UpgradeValueBy(1);
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );
}

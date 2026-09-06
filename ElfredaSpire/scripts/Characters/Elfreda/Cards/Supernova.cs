// | 超新星 | Supernova | 攻击 | 3 | 造成18/22点伤害。消耗所有[星]，每消耗1层，对所有敌人造成4/5点伤害一次。消耗。 |

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
public class Supernova : ModCardTemplate
{
    public Supernova() : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
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

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<StarElfredaPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(18, ValueProp.Move), new DamageVar("StarDamage", 4, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target!).Execute(choiceContext);

        StarElfredaPower? star = Owner.Creature.GetPower<StarElfredaPower>();
        if (star is null)
        {
            return;
        }

        int starAmount = star.Amount;
        if (starAmount <= 0) { return; }
        await PowerCmd.ModifyAmount(choiceContext, star, -starAmount, Owner.Creature, cardPlay.Card);
        await DamageCmd.Attack(DynamicVars["StarDamage"].BaseValue)
            .WithHitCount(starAmount)
            .FromCard(this)
            .TargetingAllOpponents(CombatState!)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
        DynamicVars["StarDamage"].UpgradeValueBy(1);
    }
}

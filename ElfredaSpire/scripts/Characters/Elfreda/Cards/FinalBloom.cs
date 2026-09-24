// | 终末之花 | FinalBloom | 攻击 | 3 | 施加9/12层[花]。若目标带有[绽放]，对其造成50/65点伤害。 |

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
public class FinalBloom : ModCardTemplate
{
    public FinalBloom() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
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
        HoverTipFactory.FromPower<FlowerElfredaPower>(),
        HoverTipFactory.FromPower<BloomPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FlowerElfredaPower>(9), new DamageVar(50, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is null) return;
        await PowerCmd.Apply<FlowerElfredaPower>(choiceContext, cardPlay.Target, DynamicVars["FlowerElfredaPower"].IntValue, Owner.Creature, cardPlay.Card);
        if (cardPlay.Target.HasPower<BloomPower>())
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FlowerElfredaPower"].UpgradeValueBy(3);
        DynamicVars.Damage.UpgradeValueBy(15);
    }
}

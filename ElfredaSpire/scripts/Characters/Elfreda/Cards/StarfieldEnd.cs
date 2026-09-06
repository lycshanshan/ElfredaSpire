// | 星空的尽头 | StarfieldEnd | 攻击 | 5 | -/保留。造成 35/45 点伤害。若你带有[闪耀]，此牌耗能为0。 |

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Models.Capabilities;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using ElfredaSpire.GeneralPowers;

namespace ElfredaSpire.Characters.Elfreda.Cards;

[RegisterCard(typeof(ElfredaCardPool))]
public class StarfieldEnd : ModCardTemplate, ICardEnergyCostContributor
{
    public StarfieldEnd() : base(5, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<GlitterPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(35, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target!).Execute(choiceContext);
    }

    public int ModifyEnergyCost(CardModel card, int currentCost, CostModifiers modifiers)
    {
        return card.Owner?.Creature.HasPower<GlitterPower>() == true ? 0 : currentCost;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(10);
        AddKeyword(CardKeyword.Retain);
    }
}

// | 星之引擎 | StarEngine | 技能 | 0 | 消耗3/2层[星]。获得2/3点能量。抽牌直到手牌上限。 |

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Commands;
using ElfredaSpire.GeneralPowers;

namespace ElfredaSpire.Characters.Elfreda.Cards;

[RegisterCard(typeof(ElfredaCardPool))]
public class StarEngine : ModCardTemplate
{
    public StarEngine() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
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

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StarElfredaPower>(3), new EnergyVar(2)];

    protected override bool IsPlayable =>
        Owner.Creature.GetPower<StarElfredaPower>()?.Amount >= DynamicVars["StarElfredaPower"].IntValue
        || Owner.Creature.HasPower<GlitterPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!Owner.Creature.HasPower<GlitterPower>())
            await PowerCmd.Apply<StarElfredaPower>(choiceContext, Owner.Creature, -DynamicVars["StarElfredaPower"].IntValue, Owner.Creature, cardPlay.Card);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
        
        if (Owner.PlayerCombatState is null) return;
        int drawCount = CardPile.MaxCardsInHand - Owner.PlayerCombatState.Hand.Cards.Count;
        await CardPileCmd.Draw(choiceContext, drawCount, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["StarElfredaPower"].UpgradeValueBy(-1);
        DynamicVars.Energy.UpgradeValueBy(1);
    }
}

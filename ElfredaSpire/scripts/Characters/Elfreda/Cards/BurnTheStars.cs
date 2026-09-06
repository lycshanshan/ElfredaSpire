// | 燃尽群星 | BurnTheStars | 技能 | 2 | -/保留。获得[闪耀]。结束当前回合，并立即获得一个额外回合。下回合结束时死亡。消耗。 |

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Commands;
using ElfredaSpire.GeneralPowers;

namespace ElfredaSpire.Characters.Elfreda.Cards;

[RegisterCard(typeof(ElfredaCardPool))]
public class BurnTheStars : ModCardTemplate
{
    private bool WasPlayedThisTurn => CombatManager.Instance.History.CardPlaysFinished.Any(
        (CardPlayFinishedEntry entry) =>
            entry.CardPlay.Card == this && entry.HappenedThisTurn(CombatState));

    public BurnTheStars() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
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

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<GlitterPower>(1), new PowerVar<BurnTheStarsPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<GlitterPower>(choiceContext, Owner.Creature, DynamicVars["GlitterPower"].IntValue, Owner.Creature, cardPlay.Card);
        await PowerCmd.Apply<BurnTheStarsPower>(choiceContext, Owner.Creature, DynamicVars["BurnTheStarsPower"].IntValue, Owner.Creature, cardPlay.Card);
        PlayerCmd.EndTurn(Owner, canBackOut: false);
    }

    public override bool ShouldTakeExtraTurn(Player player)
    {
        return player == Owner && WasPlayedThisTurn;
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}

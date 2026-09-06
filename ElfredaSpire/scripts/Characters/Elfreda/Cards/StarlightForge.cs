// | 星光铸造 | StarlightForge | 技能 | 1 | 获得1层[星]3/4次。消耗。 |

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
public class StarlightForge : ModCardTemplate
{
    public StarlightForge() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<StarElfredaPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StarElfredaPower>(1), new IntVar("GetCount", 3)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        for (int i = 0; i < DynamicVars["GetCount"].IntValue; i++)
        {
            await PowerCmd.Apply<StarElfredaPower>(choiceContext, Owner.Creature, DynamicVars["StarElfredaPower"].IntValue, Owner.Creature, cardPlay.Card);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["GetCount"].UpgradeValueBy(1);
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{ElfredaCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
}

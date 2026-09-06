// | 星光辐射 | StarlightRadiance | 技能 | 0 | 消耗2层[星]。对全体敌方单位施加2/3层虚弱和易伤。 |

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
public class StarlightRadiance : ModCardTemplate
{
    public StarlightRadiance() : base(0, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies, true)
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
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<WeakPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StarElfredaPower>(-2), new PowerVar<VulnerablePower>(2), new PowerVar<WeakPower>(2)];

    protected override bool IsPlayable =>
        Owner.Creature.GetPower<StarElfredaPower>()?.Amount >= -DynamicVars["StarElfredaPower"].IntValue;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StarElfredaPower>(choiceContext, Owner.Creature, DynamicVars["StarElfredaPower"].IntValue, Owner.Creature, cardPlay.Card);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target!, DynamicVars["VulnerablePower"].IntValue, Owner.Creature, cardPlay.Card);
        await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target!, DynamicVars["WeakPower"].IntValue, Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["VulnerablePower"].UpgradeValueBy(1);
        DynamicVars["WeakPower"].UpgradeValueBy(1);
    }
}

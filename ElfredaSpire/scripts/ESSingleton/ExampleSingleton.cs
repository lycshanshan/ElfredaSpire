using ElfredaSpire.ElfredaSpire.CardTags;
using ElfredaSpire.GeneralPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Models;

namespace ElfredaSpire.ESSingleton;

[RegisterSingleton]
public class ExampleSingleton : HookedSingletonModel
{
    public ExampleSingleton() : base(HookType.Combat)
    {
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return;
    }
}

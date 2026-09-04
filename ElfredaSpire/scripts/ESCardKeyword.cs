using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace ElfredaSpire.Scripts;

[RegisterOwnedCardKeyword(nameof(Example),  CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]

public class ESKeywords
{
    public static readonly CardKeyword Example = ModContentRegistry.GetQualifiedKeywordId(Entry.ModId, nameof(Example)).GetModCardKeyword();
}
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.CardTags;
using STS2RitsuLib.Content;

namespace ElfredaSpire.ElfredaSpire.CardTags;

[RegisterOwnedCardTag(nameof(Example))]
public static class ESCardTag
{
    public static readonly CardTag Example =
        ModContentRegistry
            .GetQualifiedCardTagId(Entry.ModId, nameof(Example))
            .GetModCardTag();
}

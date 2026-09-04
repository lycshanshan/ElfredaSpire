using ElfredaSpire.Characters.Elfreda.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using STS2RitsuLib.Scaffolding.Content;

#nullable enable
namespace ElfredaSpire.Characters.Events;

public class ElfredaRestSiteOption(Player owner) : ModRestSiteOptionTemplate(owner)
{
    private const int CountIncrease = 3;
    private const string IconPath = $"{Entry.ResPath}/images/characters/Elfreda/restsite_icon_Elfreda.png";

    public override string OptionId => "ELFREDA_STAR_AND_FLOWER";

    public override RestSiteOptionAssetProfile AssetProfile => new(IconPath: IconPath);

    public override LocString CustomTitle => new("events", "ELFREDA_REST_SITE_OPTION.title");

    public override LocString Description
    {
        get
        {
            LocString description = new("events", "ELFREDA_REST_SITE_OPTION.description");
            description.Add("Count", CountIncrease);
            return description;
        }
    }

    public override Task<bool> OnSelect()
    {
        StarAndFlower? starAndFlower = Owner.GetRelic<StarAndFlower>();
        StarCloudAndFlowerSea? starCloudAndFlowerSea = Owner.GetRelic<StarCloudAndFlowerSea>();

        if (starAndFlower is not null)
        {
            starAndFlower.Count += CountIncrease;
            starAndFlower.Flash();
        }

        if (starCloudAndFlowerSea is not null)
        {
            starCloudAndFlowerSea.Count += CountIncrease;
            starCloudAndFlowerSea.Flash();
        }

        return Task.FromResult(starAndFlower is not null || starCloudAndFlowerSea is not null);
    }
}

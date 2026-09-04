using System.Reflection;
using ElfredaSpire.Characters.Elfreda.Relics;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop;

namespace ElfredaSpire;

[ModInitializer(nameof(Init))]
public class Entry
{
    // 你的modid
    public const string ModId = "ElfredaSpire";
    public const string ResPath = $"res://{ModId}";
    private const string BuildMarker = "ElfredaSpire loaded vision 0.0.0";
    public static readonly Logger Logger = RitsuLibFramework.CreateLogger(ModId);
    

    public static void Init()
    {
        // harmony可用，但是最好用ritsu的封装patch，见补丁系统一章
        // var harmony = new Harmony("com.example.testmod");
        // harmony.PatchAll();
        var assembly = Assembly.GetExecutingAssembly();
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);
        // 自动注册内容
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);
        RitsuLibFramework.RegisterTouchOfOrobasRefinementMapping<StarAndFlower, StarCloudAndFlowerSea>();
        // RitsuLibFramework.RegisterArchaicToothTranscendenceMapping<Radiance, HolyFire>();

        // ModContentRegistry.For(ModId)
        //     .RegisterCardLibraryCompendiumSharedPoolFilter<GeneralCardPool>(
        //         "reme_multiclass_shared_pool", // ID
        //         "res://ElfredaSpire/images/characters/Elfreda/character_icon_Elfreda.png" // 图标位置
        //         // null // 放置顺序（可选）
        //     );
        Logger.Info(BuildMarker);
        
        var cardTags = RitsuLibFramework.GetCardTagRegistry(Entry.ModId);
        cardTags.RegisterOwned("heavy");
    }
}

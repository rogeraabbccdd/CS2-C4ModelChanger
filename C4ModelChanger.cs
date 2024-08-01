using System.Text.Json.Serialization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Core.Attributes.Registration;

namespace C4ModelChangerPlugin;

public class Config : BasePluginConfig
{
    [JsonPropertyName("ConfigVersion")]
    public override int Version { get; set; } = 1;

    [JsonPropertyName("Model")]
    public string Model { get; set; } = "";

    [JsonPropertyName("Size")]
    public float Scale { get; set; } = 1.0f;
}

public class Plugin : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "C4 Model Changer";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Kento";
    public override string ModuleDescription => "Change C4 Model.";
    public Config Config { get; set; } = new();

    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnServerPrecacheResources>(OnPrecacheResources);
    }
    public override void Unload(bool hotReload)
    {
        RemoveListener<Listeners.OnServerPrecacheResources>(OnPrecacheResources);
    }
    public void OnConfigParsed(Config config)
    {
        Config = config;
    }
    public void OnPrecacheResources (ResourceManifest manifest)
    {
        manifest.AddResource(Config.Model);
    }

    [GameEventHandler]
    public HookResult OnEventBombPlanted(EventBombPlanted @event, GameEventInfo info)
    {
        var c4 = Utilities.FindAllEntitiesByDesignerName<CPlantedC4>("planted_c4").First();
        if (c4 == null || c4.Entity == null || !c4.IsValid) return HookResult.Continue;

        if (Config.Model.Length == 0)   return HookResult.Continue;
        c4.SetModel(Config.Model);
        c4.Entity.EntityInstance.AddEntityIOEvent("SetScale", null, null, Config.Scale.ToString());

        return HookResult.Continue;
    }
}

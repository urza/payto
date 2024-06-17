using System.Text.Json.Serialization;

namespace payto.JsonTypes;


[JsonSerializable(typeof(ClnPluginList))]
public class ClnPluginList
{
    public string command { get; set; }
    public Plugin[] plugins { get; set; }
}

[JsonSerializable(typeof(Plugin))]
public class Plugin
{
    public string name { get; set; }
    public bool active { get; set; }
    public bool dynamic { get; set; }
}

[JsonSerializable(typeof(ClnCurrencyconvertResult))]
public class ClnCurrencyconvertResult
{
    public string msat { get; set; }
}

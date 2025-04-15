public class AppDataMigrator : DataMigrator<AppData>
{
    public override AppData Migrate(string json, int oldVersion, int newVersion)
    {
        // migration example
        var jo = Newtonsoft.Json.Linq.JObject.Parse(json);

        if (oldVersion == 1 && newVersion == 2)
        {
            jo["NewField"] = "default_value";
            jo["Version"] = newVersion;
        }
        
        return jo.ToObject<AppData>();
    }
}


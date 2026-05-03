using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace DnD5eBattleApp;

public class SchemaExporter {

    public static JsonSerializerOptions Options {get; set;} = new JsonSerializerOptions
    {
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        },
        TypeInfoResolver = new DefaultJsonTypeInfoResolver()
    };

    public static void ExtractSchema(Type type, string outputPath)
    {
        JsonNode schema = Options.GetJsonSchemaAsNode(type);

        // Write to file
        string jsonString = schema.ToJsonString(Options);
        File.WriteAllText(outputPath, jsonString);
    }
    public static void ExtractSchemas(string folderPath)
    {
        ExtractSchema(typeof(WeaponSpec), Path.Combine(folderPath, "WeaponSpecSchema.json"));
        ExtractSchema(typeof(MonsterSpec), Path.Combine(folderPath, "MonsterSpecSchema.json"));
        ExtractSchema(typeof(SpellSpec), Path.Combine(folderPath, "SpellSpecSchema.json"));
    }
}

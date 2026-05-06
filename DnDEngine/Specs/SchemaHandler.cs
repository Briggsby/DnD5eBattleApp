using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace DnD5eBattleApp;

public class SchemaHandler {

    public static JsonSerializerOptions Options {get; set;} = new JsonSerializerOptions
    {
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        },
        TypeInfoResolver = new DefaultJsonTypeInfoResolver()
    };

    public static JsonSchemaExporterOptions SchemaExporterOptions {get; set;} = new JsonSchemaExporterOptions
    {
        TransformSchemaNode = (context, schema) => {
            // Determine if a type or property and extract the relevant attribute provider.
            ICustomAttributeProvider attributeProvider = context.PropertyInfo is not null
                ? context.PropertyInfo.AttributeProvider
                : context.TypeInfo.Type;

            // Look up any description attributes.
            SchemaRefAttribute schemaRefAttr = attributeProvider
                .GetCustomAttributes(inherit: true)
                .Select(attr => attr as SchemaRefAttribute)
                .FirstOrDefault(attr => attr is not null);

            if (schemaRefAttr is null) {
                return schema;
            }
            JsonObject jObj = schema as JsonObject;
            JsonObject newJObj = new JsonObject();

            JsonObject refObj = new JsonObject();
            refObj["$ref"] = schemaRefAttr.SchemaRef;

            bool optional = (jObj["type"] as JsonArray).Any(node => node.ToString() == "null");

            if ((jObj["type"] as JsonArray).Any(node => node.ToString() == "string")) {
                if (optional) {
                    newJObj["anyOf"] = new JsonArray { refObj, new JsonObject { ["type"] = "null" } };
                } else {
                    newJObj["anyOf"] = new JsonArray { refObj };
                }
            } else if ((jObj["type"] as JsonArray).Any(node => node.ToString() == "array")) {
                if (optional) {
                    newJObj["type"] = new JsonArray { "array", "null" };
                } else {
                    newJObj["type"] = new JsonArray { "array" };
                }
                newJObj["items"] = refObj;
            } else {
                throw new InvalidOperationException("Unsupported type: " + (jObj["type"] as JsonArray).ToString());
            }
            return newJObj;
        }
    };

    public static void ExtractSchema(Type type, string outputPath)
    {
        JsonNode schema = Options.GetJsonSchemaAsNode(type, SchemaExporterOptions);

        // Write to file
        string jsonString = schema.ToJsonString(Options);
        File.WriteAllText(outputPath, jsonString);
    }
    public static void ExtractSchemas(string folderPath)
    {
        ExtractSchema(typeof(WeaponSpec), Path.Combine(folderPath, "WeaponSpecSchema.json"));
        ExtractSchema(typeof(MonsterSpec), Path.Combine(folderPath, "MonsterSpecSchema.json"));
        ExtractSchema(typeof(SpellSpec), Path.Combine(folderPath, "SpellSpecSchema.json"));
        ExtractSchema(typeof(ConditionSpec), Path.Combine(folderPath, "ConditionSpecSchema.json"));
    }

    public static void IngestSpecs<T>(string path, Dictionary<string, T> dict)
    {
        // Iterate through every file in path, parse it as the given type, and add it to the given dictionary with the spec's name as the key
        string[] files = System.IO.Directory.GetFiles(path);
        foreach (string file in files)
        {
            string json = System.IO.File.ReadAllText(file);
            T spec = System.Text.Json.JsonSerializer.Deserialize<T>(json, Options);
            dict.Add((string)typeof(T).GetProperty("Name").GetValue(spec), spec);
        }
    }

}

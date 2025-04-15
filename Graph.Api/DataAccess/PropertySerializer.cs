using System.Text.Json;

namespace Graph.Api.DataAccess;

public static class PropertySerializer
{
    public static string SerializeProperties<T>(this T obj) where T : class
    {
        if (obj is Vertex vertex)
        {
            return vertex.Properties.SerializeProperties();
        }

        var properties = typeof(T).GetProperties();
        var serializedProperties = new List<string>();

        foreach (var property in properties)
        {
            var propertyName = property.Name.ToCamelCase();
            var value = property.GetValue(obj);

            serializedProperties.AddProperty(propertyName, value);
        }

        return serializedProperties.Serialize();
    }

    private static string SerializeProperties(this Dictionary<string, object> properties)
    {
        var serializedProperties = new List<string>();

        foreach (var kvp in properties)
        {
            var propertyName = kvp.Key.ToCamelCase();
            var value = kvp.Value;

            serializedProperties.AddProperty(propertyName, value);
        }

        return serializedProperties.Serialize();
    }

    private static void AddProperty(this List<string> serializedProperties, string propertyName, object value)
    {
        if (value != null)
        {
            switch (value)
            {
                case string strValue:
                    serializedProperties.Add($"{propertyName}: '{strValue.Replace("'", "''")}'");
                    break;
                case int intValue:
                    serializedProperties.Add($"{propertyName}: {intValue}");
                    break;
                case double doubleValue:
                    serializedProperties.Add($"{propertyName}: {doubleValue}");
                    break;
                case float floatValue:
                    serializedProperties.Add($"{propertyName}: {floatValue}");
                    break;
                case decimal decimalValue:
                    serializedProperties.Add($"{propertyName}: {decimalValue}");
                    break;
                case bool boolValue:
                    serializedProperties.Add($"{propertyName}: {boolValue.ToString().ToLower()}");
                    break;
                case JsonElement jsonElement:
                    // Handle JsonElement serialization
                    switch (jsonElement.ValueKind)
                    {
                        case JsonValueKind.Object:
                            var jsonObject = jsonElement.ToString();
                            serializedProperties.Add($"{propertyName}: {jsonObject}");
                            break;
                        case JsonValueKind.Array:
                            var jsonArray = jsonElement.ToString();
                            serializedProperties.Add($"{propertyName}: {jsonArray}");
                            break;
                        case JsonValueKind.String:
                            var jsonString = jsonElement.GetString().Replace("'", "''");
                            serializedProperties.Add($"{propertyName}: '{jsonString}'");
                            break;
                        case JsonValueKind.Number:
                            try
                            {
                                var jsonInt = jsonElement.GetInt32();
                                serializedProperties.Add($"{propertyName}: {jsonInt}");
                            }
                            catch (JsonException)
                            {
                                // If it fails to parse as int, try double
                                var jsonDouble = jsonElement.GetDouble();
                                serializedProperties.Add($"{propertyName}: {jsonDouble}");
                            }
                            var jsonNumber = jsonElement.GetDouble();
                            serializedProperties.Add($"{propertyName}: {jsonNumber}");
                            break;
                        case JsonValueKind.True:
                        case JsonValueKind.False:
                            var jsonBool = jsonElement.GetBoolean();
                            serializedProperties.Add($"{propertyName}: {jsonBool.ToString().ToLower()}");
                            break;
                        default:
                            // Handle other cases if needed
                            break;
                    }
                    break;
                // Add more cases for other types as needed
                default:
                    // For unsupported types, you can choose to skip or handle them differently
                    break;
            }
        }
    }

    private static string Serialize(this List<string> serializedProperties)
    {
        if (serializedProperties.Count == 0)
        {
            return string.Empty;
        }
        else
        {
            return "{" + string.Join(", ", serializedProperties) + "}";
        }
    }

    private static string ToCamelCase(this string str)
    {
        if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
            return str;

        return char.ToLowerInvariant(str[0]) + str.Substring(1);
    }
}
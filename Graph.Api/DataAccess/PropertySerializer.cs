using System.Text.Json;

namespace Graph.Api.DataAccess;

public class PropertySerializer
{
    public static string SerializeProperties<T>(T obj)
    {
        var properties = typeof(T).GetProperties();
        var serializedProperties = new List<string>();

        foreach (var property in properties)
        {
            var propertyName = ToCamelCase(property.Name);
            var value = property.GetValue(obj);
            if (value != null)
            {
                switch (value)
                {
                    case string strValue:
                        serializedProperties.Add($"{propertyName}: '{strValue}'");
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
                    // Add more cases for other types as needed
                    default:
                        // For unsupported types, you can choose to skip or handle them differently
                        break;
                }
            }
        }

        return "{" + string.Join(", ", serializedProperties) + "}";
    }

    public static string SerializeJsonDocument(JsonDocument jsonDocument)
    {
        var properties = new List<string>();
        foreach (var property in jsonDocument.RootElement.EnumerateObject())
        {
            var propertyName = ToCamelCase(property.Name);
            var value = property.Value.ToString();
            properties.Add($"{propertyName}: '{value}'");
        }

        return "{" + string.Join(", ", properties) + "}";
    }

    private static string ToCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
            return str;

        return char.ToLowerInvariant(str[0]) + str.Substring(1);
    }
}
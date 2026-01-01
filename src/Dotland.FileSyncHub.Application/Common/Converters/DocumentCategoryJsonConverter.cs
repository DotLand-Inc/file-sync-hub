using Dotland.FileSyncHub.Domain.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dotland.FileSyncHub.Application.Common.Converters;

/// <summary>
/// JSON converter for DocumentCategory that accepts both integer and string values
/// </summary>
public class DocumentCategoryJsonConverter : JsonConverter<DocumentCategory>
{
    public override DocumentCategory Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Number:
                // Handle integer values (e.g., 1, 2, 3)
                if (reader.TryGetInt32(out var intValue))
                {
                    if (Enum.IsDefined(typeof(DocumentCategory), intValue))
                    {
                        return (DocumentCategory)intValue;
                    }
                    throw new JsonException($"Invalid DocumentCategory value: {intValue}");
                }
                throw new JsonException("Invalid number format for DocumentCategory");

            case JsonTokenType.String:
                // Handle string values (e.g., "Invoices", "Contracts")
                var stringValue = reader.GetString();
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    throw new JsonException("DocumentCategory string value cannot be empty");
                }

                // Try to parse as enum name (case-insensitive)
                if (Enum.TryParse<DocumentCategory>(stringValue, ignoreCase: true, out var enumValue))
                {
                    return enumValue;
                }

                // Try to parse as integer string (e.g., "1", "2")
                if (int.TryParse(stringValue, out var intFromString))
                {
                    if (Enum.IsDefined(typeof(DocumentCategory), intFromString))
                    {
                        return (DocumentCategory)intFromString;
                    }
                }

                throw new JsonException($"Invalid DocumentCategory value: '{stringValue}'. Valid values are: {string.Join(", ", Enum.GetNames<DocumentCategory>())}");

            default:
                throw new JsonException($"Unexpected token type for DocumentCategory: {reader.TokenType}");
        }
    }

    public override void Write(Utf8JsonWriter writer, DocumentCategory value, JsonSerializerOptions options)
    {
        // Write as string by default for better readability
        writer.WriteStringValue(value.ToString());
    }
}

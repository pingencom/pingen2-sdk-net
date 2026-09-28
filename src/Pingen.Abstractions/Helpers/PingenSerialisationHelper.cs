using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Pingen.Abstractions.Attributes;
using Pingen.Abstractions.Helpers.JsonConverters;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Base;

namespace Pingen.Abstractions.Helpers;

/// <summary>
/// Helper for JSON serialisation of Pingen API data
/// </summary>
public static class PingenSerialisationHelper
{
    /// <summary>
    /// Cached JSON serializer options with default settings and custom converters.
    /// Thread-safe once initialized; do not mutate.
    /// </summary>
    private static readonly JsonSerializerOptions CachedSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        // JSON is only ever sent as an HTTP request body (never embedded in HTML), so relaxed escaping is safe and
        // keeps the wire clean: the datetime offset's '+' is emitted literally rather than as a \uXXXX escape, and
        // non-ASCII recipient data is emitted as UTF-8.
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new PingenDateTimeConverter(), new PingenDateTimeNullableConverter(), new PingenKeyValuePairStringObjectConverter() },
    };

    /// <summary>
    /// Parses the text representing a single JSON value into an instance of the type specified by a generic type parameter.
    /// </summary>
    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, options: CachedSerializerOptions);
    }

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    public static async Task<T?> DeserializeAsync<T>(Stream utf8Json)
    {
        return await JsonSerializer.DeserializeAsync<T>(utf8Json, options: CachedSerializerOptions);
    }

    /// <summary>
    /// Converts the value of a type specified by a generic type parameter into a JSON string.
    /// </summary>
    public static string Serialize(object data)
    {
        return JsonSerializer.Serialize(data, options: CachedSerializerOptions);
    }

    /// <summary>
    /// Maps each JSON:API <c>type</c> discriminator (<c>type</c>) to its .NET attributes
    /// type. Used by <see cref="TryGetIncludedData{T}"/> to resolve <c>included</c> resources back to a
    /// strongly-typed <see cref="Data{T}"/>. Request-only discriminators are not deserialized and so are absent.
    /// </summary>
    public static IReadOnlyDictionary<string, Type> PingenApiDataTypeMapping => DataTypeMapping;

    private static readonly Dictionary<string, Type> DataTypeMapping = BuildDataTypeMapping();

    /// <summary>
    /// Builds the <c>type</c> discriminator to .NET type map by scanning the abstractions assembly for models
    /// decorated with <see cref="PingenApiTypeAttribute"/>. This removes the hand-maintained table: a new
    /// deserializable model only needs the attribute, nothing here.
    /// </summary>
    private static Dictionary<string, Type> BuildDataTypeMapping()
    {
        var mapping = new Dictionary<string, Type>(StringComparer.Ordinal);

        foreach (var type in typeof(PingenApiTypeAttribute).Assembly.GetTypes())
        {
            var attribute = type.GetCustomAttribute<PingenApiTypeAttribute>();
            if (attribute is null)
                continue;

            foreach (var discriminator in attribute.Types)
                mapping[discriminator] = type;
        }

        return mapping;
    }

    /// <summary>
    /// Attempts to extract a single included resource of type <typeparamref name="T"/> from the data result.
    /// Uses <see cref="PingenApiDataTypeMapping"/> to resolve the JSON:API <c>type</c> discriminator
    /// to the .NET attributes type.
    /// </summary>
    /// <param name="dataResult">The data result containing the <c>included</c> collection.</param>
    /// <param name="included">When this method returns, contains the deserialized <see cref="Data{T}"/> if found; otherwise <c>null</c>.</param>
    /// <typeparam name="T">The attributes type to search for (must implement <see cref="IAttributes"/>).</typeparam>
    /// <returns><c>true</c> if a matching included resource was found; otherwise <c>false</c>.</returns>
    public static bool TryGetIncludedData<T>(IDataResult dataResult, out Data<T>? included)
        where T : IAttributes
    {
        included = dataResult.Included?.OfType<T>().SingleOrDefault();
        return included is not null;
    }
}

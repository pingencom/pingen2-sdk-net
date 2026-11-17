using System.Text.Json;
using System.Text.Json.Serialization;
using Pingen.Abstractions.Helpers;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Base;

namespace Pingen.Abstractions.Models.Api.Embedded.DataResults;

/// <summary>
/// A strongly-typed wrapper around the JSON:API <c>included</c> array.
/// Stores the raw <see cref="JsonElement"/> items and provides typed access
/// via <see cref="OfType{T}"/> and <see cref="FindById{T}"/>.
/// </summary>
[JsonConverter(typeof(IncludedCollectionJsonConverter))]
public sealed class IncludedCollection
{
    /// <summary>
    /// A reusable empty <see cref="IncludedCollection"/> singleton.
    /// </summary>
    public static IncludedCollection Empty { get; } = new([]);

    /// <summary>
    /// The raw JSON elements from the <c>included</c> array.
    /// </summary>
    public IReadOnlyList<JsonElement> RawItems { get; }

    /// <summary>
    /// The number of raw items in the collection.
    /// </summary>
    public int Count => RawItems.Count;

    /// <summary>
    /// Initializes a new <see cref="IncludedCollection"/> from a list of raw JSON elements.
    /// </summary>
    /// <param name="rawItems">The JSON elements from the <c>included</c> array.</param>
    public IncludedCollection(IReadOnlyList<JsonElement> rawItems)
    {
        RawItems = rawItems;
    }

    /// <summary>
    /// Returns all included resources whose <c>type</c> maps to <typeparamref name="T"/>
    /// in <see cref="PingenSerialisationHelper.PingenApiDataTypeMapping"/>,
    /// deserialized as <see cref="Data{TAttributes}"/>.
    /// <para>
    /// A single resource can map to <typeparamref name="T"/> under more than one discriminator (Pingen emits a
    /// delivery event as both <c>letters_events</c> and <c>deliverables_events</c> with a shared <c>id</c>);
    /// such cross-discriminator repeats are collapsed to their first occurrence. Same-discriminator duplicates
    /// and id-less resources are returned as-is.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The attributes type to filter by (must implement <see cref="IAttributes"/>).</typeparam>
    /// <returns>An enumerable of <see cref="Data{TAttributes}"/> for each matching included resource.</returns>
    public IEnumerable<Data<T>> OfType<T>()
        where T : IAttributes
    {
        var mapping = PingenSerialisationHelper.PingenApiDataTypeMapping;
        var discriminatorById = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var element in RawItems)
        {
            if (!element.TryGetProperty("type", out var typeProperty))
                continue;

            if (typeProperty.GetString() is not { } typeString)
                continue;

            if (!mapping.TryGetValue(typeString, out var mappedType) || mappedType != typeof(T))
                continue;

            if (element.TryGetProperty("id", out var idProperty) && idProperty.GetString() is { } idString)
            {
                if (discriminatorById.TryGetValue(idString, out var seenType) && seenType != typeString)
                    continue;

                discriminatorById[idString] = typeString;
            }

            var deserialized = PingenSerialisationHelper.Deserialize<Data<T>>(element.GetRawText());
            if (deserialized is not null)
                yield return deserialized;
        }
    }

    /// <summary>
    /// Finds the first included resource whose <c>type</c> maps to <typeparamref name="T"/>
    /// and whose <c>id</c> matches <paramref name="id"/>.
    /// </summary>
    /// <typeparam name="T">The attributes type to filter by (must implement <see cref="IAttributes"/>).</typeparam>
    /// <param name="id">The resource identifier to match.</param>
    /// <returns>The matching <see cref="Data{TAttributes}"/> or <c>null</c> if not found.</returns>
    public Data<T>? FindById<T>(string id)
        where T : IAttributes
    {
        var mapping = PingenSerialisationHelper.PingenApiDataTypeMapping;

        foreach (var element in RawItems)
        {
            if (!element.TryGetProperty("type", out var typeProperty))
                continue;

            if (typeProperty.GetString() is not { } typeString)
                continue;

            if (!mapping.TryGetValue(typeString, out var mappedType) || mappedType != typeof(T))
                continue;

            if (!element.TryGetProperty("id", out var idProperty))
                continue;

            if (idProperty.GetString() != id)
                continue;

            return PingenSerialisationHelper.Deserialize<Data<T>>(element.GetRawText());
        }

        return null;
    }
}

/// <summary>
/// Custom JSON converter for <see cref="IncludedCollection"/>.
/// Reads a JSON array of objects into a list of <see cref="JsonElement"/> items,
/// and writes them back as a JSON array.
/// </summary>
internal sealed class IncludedCollectionJsonConverter : JsonConverter<IncludedCollection>
{
    /// <inheritdoc />
    public override IncludedCollection? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // A JSON null (e.g. "included": null) is handled by the serializer itself and never reaches this method.
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Expected start of JSON array for included collection.");

        var items = new List<JsonElement>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            var element = JsonElement.ParseValue(ref reader);
            items.Add(element);
        }

        return new IncludedCollection(items);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IncludedCollection value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var item in value.RawItems)
        {
            item.WriteTo(writer);
        }
        writer.WriteEndArray();
    }
}

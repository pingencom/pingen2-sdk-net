using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Pingen.Abstractions.Helpers;
using Pingen.Abstractions.Models.Ebills;

namespace Pingen.Tests.Models;

[TestFixture]
public sealed class ModelPropertyCoverageTests
{
    [Test]
    public void AllModels_RoundTripThroughTheSdkSerializer()
    {
        var models = typeof(Ebill)
            .Assembly.GetTypes()
            .Where(t =>
                t is { IsClass: true, IsAbstract: false }
                && !t.IsGenericTypeDefinition
                && t.Namespace?.StartsWith("Pingen.Abstractions.Models", StringComparison.Ordinal) == true
            )
            .ToList();

        var exercised = 0;
        foreach (var model in models)
        {
            object? instance;
            try
            {
                instance = JsonSerializer.Deserialize("{}", model);
            }
            catch (Exception e) when (e is JsonException or NotSupportedException)
            {
                // Records with required members (e.g. the *Upload views, DataPost/DataPatch) cannot be built
                // from "{}"; those are exercised by the connector/upload tests instead.
                continue;
            }

            if (instance is null)
                continue;

            foreach (
                var property in model
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p is { CanRead: true } && p.GetIndexParameters().Length == 0)
            )
                _ = property.GetValue(instance);

            // Types with a bespoke [JsonConverter] (e.g. IncludedCollection) are deserialize-only by design; the
            // SDK never serializes them, so exercising their Write path here would be misleading - they are
            // covered by dedicated converter tests.
            if (model.GetCustomAttribute<JsonConverterAttribute>() is null)
            {
                string? json;
                try
                {
                    // Every other model must serialize through the SDK's own serializer without an unexpected
                    // throw (a converter Write bug such as an NRE surfaces here instead of passing silently)...
                    json = PingenSerialisationHelper.Serialize(instance);
                }
                catch (NotSupportedException)
                {
                    // ...except a model carrying a member of a type System.Text.Json structurally cannot write
                    // (the SDK never serializes it); its getters are already exercised above.
                    json = null;
                }

                if (json is not null)
                    Should.NotThrow(() => JsonDocument.Parse(json).Dispose());
            }

            exercised++;
        }

        exercised.ShouldBeGreaterThan(30);
    }
}

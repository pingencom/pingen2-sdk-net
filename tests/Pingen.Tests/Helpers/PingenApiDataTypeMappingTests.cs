using Pingen.Abstractions.Helpers;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Tests.Helpers;

public class PingenApiDataTypeMappingTests
{
    [Test]
    public void PingenApiDataTypeMapping_AllMappedTypes_AreNonNullAndImplementIAttributes()
    {
        IReadOnlyDictionary<string, Type> mapping = PingenSerialisationHelper.PingenApiDataTypeMapping;

        mapping.ShouldSatisfyAllConditions(
            mapping
                .Select<KeyValuePair<string, Type>, Action>(kvp =>
                    () =>
                    {
                        kvp.Value.ShouldNotBeNull($"Mapping value for '{kvp.Key}' must not be null.");
                        typeof(IAttributes)
                            .IsAssignableFrom(kvp.Value)
                            .ShouldBeTrue($"Mapping for '{kvp.Key}' resolves to {kvp.Value.FullName} which does not implement IAttributes.");
                    }
                )
                .ToArray()
        );
    }

    [Test]
    public void PingenApiDataTypeMapping_ResolvesCoreTypes()
    {
        IReadOnlyDictionary<string, Type> mapping = PingenSerialisationHelper.PingenApiDataTypeMapping;

        mapping.ShouldSatisfyAllConditions(
            () => mapping.ContainsKey("letters").ShouldBeTrue(),
            () => mapping.ContainsKey("batches").ShouldBeTrue(),
            () => mapping.ContainsKey("organisations").ShouldBeTrue(),
            () => mapping["deliverables_events"].ShouldBe(mapping["letters_events"])
        );
    }
}

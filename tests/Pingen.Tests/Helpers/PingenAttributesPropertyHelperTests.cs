using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json.Serialization;
using Pingen.Abstractions.Helpers;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Letters;

namespace Pingen.Tests.Helpers;

public class PingenAttributesPropertyHelperTests
{
    [Test]
    public void GetJsonPropertyName_LetterStatus_ReturnsCorrectName()
    {
        string result = PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(l => l.Status);

        result.ShouldBe("status");
    }

    [Test]
    public void GetJsonPropertyName_LetterFileOriginalName_ReturnsSnakeCase()
    {
        string result = PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(l => l.FileOriginalName);

        result.ShouldBe("file_original_name");
    }

    [Test]
    public void GetJsonPropertyName_LetterCountry_ReturnsCorrectName()
    {
        string result = PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(l => l.Country);

        result.ShouldBe("country");
    }

    [Test]
    public void GetJsonPropertyName_NonMemberExpression_ThrowsInvalidOperationException()
    {
        Should.Throw<InvalidOperationException>(() => PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(l => "constant"));
    }

    [Test]
    public void GetJsonPropertyName_PropertyMissingJsonPropertyName_ThrowsInvalidOperationException()
    {
        Should.Throw<InvalidOperationException>(() => PingenAttributesPropertyHelper<AttrFixtureMissingJsonName>.GetJsonPropertyName(x => x.PlainProperty));
    }

    [Test]
    public void GetJsonPropertyName_PropertyWithMultipleUnrelatedAttributes_ReturnsJsonPropertyName()
    {
        string result = PingenAttributesPropertyHelper<AttrFixtureWithMixedAttributes>.GetJsonPropertyName(x => x.Name);

        result.ShouldBe("real_name");
    }

    [Test]
    public void GetJsonPropertyName_MethodCallExpression_ThrowsInvalidOperationException()
    {
        Should.Throw<InvalidOperationException>(() => PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(l => l.Status!.ToUpper()));
    }

    [Test]
    public void GetJsonPropertyName_BinaryExpression_ThrowsInvalidOperationException()
    {
        Should.Throw<InvalidOperationException>(() => PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(l => l.Status + "suffix"));
    }

    [Test]
    public void JsonPropertyNameAttribute_DoesNotAllowMultipleOnSingleProperty()
    {
        AttributeUsageAttribute? usage = typeof(JsonPropertyNameAttribute).GetCustomAttribute<AttributeUsageAttribute>();

        usage.ShouldNotBeNull();
        usage!.AllowMultiple.ShouldBeFalse();
    }

    [Test]
    public void GetJsonPropertyName_LetterCreatedAt_ReturnsSnakeCase()
    {
        string result = PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(l => l.CreatedAt);

        result.ShouldBe("created_at");
    }

    [Test]
    public void GetJsonPropertyName_LetterPriceCurrency_ReturnsSnakeCase()
    {
        string result = PingenAttributesPropertyHelper<Letter>.GetJsonPropertyName(l => l.PriceCurrency);

        result.ShouldBe("price_currency");
    }

    private sealed record AttrFixtureMissingJsonName(string PlainProperty) : IAttributes;

    private sealed record AttrFixtureWithMixedAttributes(
        [property: JsonPropertyName("real_name")]
        [property: Required]
        [property: System.ComponentModel.Description("a property carrying multiple unrelated attributes")]
            string Name
    ) : IAttributes;
}

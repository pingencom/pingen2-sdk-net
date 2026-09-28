using System.Text.Json;
using Pingen.Abstractions.Interfaces.Data;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Base.Embedded;
using Pingen.Abstractions.Models.Letters.Embedded;

namespace Pingen.Tests.Models;

[TestFixture]
public sealed class RemainingModelCoverageTests
{
    private sealed record TestAbilities : IAbilities;

    [Test]
    public void LetterMetaData_ExposesRecipientSenderAndContactFields()
    {
        var contact = new LetterMetaDataContact
        {
            Name = "Jane Doe",
            Street = "Bahnhofstrasse",
            Number = "1",
            Zip = "8001",
            City = "Zürich",
            Country = "CH",
        };
        var metaData = new LetterMetaData { Recipient = contact, Sender = contact };

        metaData.ShouldSatisfyAllConditions(
            () => metaData.Recipient.Name.ShouldBe("Jane Doe"),
            () => metaData.Sender.Country.ShouldBe("CH"),
            () => contact.Street.ShouldBe("Bahnhofstrasse"),
            () => contact.Number.ShouldBe("1"),
            () => contact.Zip.ShouldBe("8001"),
            () => contact.City.ShouldBe("Zürich")
        );
    }

    [Test]
    public void MetaAbilityWithOrganisation_ExposesSelfAndOrganisation()
    {
        var self = new TestAbilities();
        var organisation = new TestAbilities();

        var meta = new MetaAbilityWithOrganisation<TestAbilities, TestAbilities>(self, organisation);

        meta.ShouldSatisfyAllConditions(() => meta.Self.ShouldBeSameAs(self), () => meta.Organisation.ShouldBeSameAs(organisation));
    }

    [Test]
    public void IncludedCollectionConverter_WhenIncludedIsNotAnArray_ThrowsJsonException()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new IncludedCollectionJsonConverter());

        Should.Throw<JsonException>(() => JsonSerializer.Deserialize<IncludedCollection>("{}", options));
    }

    [Test]
    public void IncludedCollectionConverter_WhenIncludedIsNull_ReturnsNull()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new IncludedCollectionJsonConverter());

        JsonSerializer.Deserialize<IncludedCollection>("null", options).ShouldBeNull();
    }
}

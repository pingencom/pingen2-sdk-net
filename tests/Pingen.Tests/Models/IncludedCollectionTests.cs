using System.Text.Json;
using Pingen.Abstractions.Helpers;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Base;
using Pingen.Abstractions.Models.LetterEvents;
using Pingen.Abstractions.Models.Letters;
using Pingen.Abstractions.Models.Organisations;
using Pingen.Abstractions.Models.UserAssociations;

namespace Pingen.Tests.Models;

public class IncludedCollectionTests
{
    [Test]
    public void Empty_HasZeroCount()
    {
        IncludedCollection empty = IncludedCollection.Empty;

        empty.Count.ShouldBe(0);
        empty.RawItems.ShouldBeEmpty();
    }

    [Test]
    public void Empty_ReturnsSameInstance()
    {
        IncludedCollection first = IncludedCollection.Empty;
        IncludedCollection second = IncludedCollection.Empty;

        ReferenceEquals(first, second).ShouldBeTrue();
    }

    [Test]
    public void Deserialize_EmptyIncludedArray_ProducesEmptyCollection()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": []
            }
            """;

        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        result.Included.ShouldNotBeNull();
        result.Included!.Count.ShouldBe(0);
    }

    [Test]
    public void Deserialize_MissingIncludedKey_ProducesNull()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } }
            }
            """;

        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        result.Included.ShouldBeNull();
    }

    [Test]
    public void Deserialize_NullIncludedValue_ProducesNull()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": null
            }
            """;

        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        result.Included.ShouldBeNull();
    }

    [Test]
    public void OfType_MatchingType_ReturnsDeserializedItems()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "org-1", "type": "organisations", "attributes": { "name": "Test Org" } },
                    { "id": "letter-2", "type": "letters", "attributes": { "status": "sent" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        var orgs = result.Included!.OfType<Organisation>().ToList();

        orgs.Count.ShouldBe(1);
        orgs[0].Id.ShouldBe("org-1");
        orgs[0].Attributes.Name.ShouldBe("Test Org");
    }

    [Test]
    public void OfType_NoMatchingType_ReturnsEmpty()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "letter-2", "type": "letters", "attributes": { "status": "sent" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        var orgs = result.Included!.OfType<Organisation>().ToList();

        orgs.ShouldBeEmpty();
    }

    [Test]
    public void OfType_UnknownTypeDiscriminator_SkipsElement()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "unknown-1", "type": "unknown_type_xyz", "attributes": {} },
                    { "id": "org-1", "type": "organisations", "attributes": { "name": "Test Org" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        var orgs = result.Included!.OfType<Organisation>().ToList();

        orgs.Count.ShouldBe(1);
        orgs[0].Id.ShouldBe("org-1");
    }

    [Test]
    public void OfType_MultipleMatchingItems_ReturnsAll()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "letter-2", "type": "letters", "attributes": { "status": "sent" } },
                    { "id": "org-1", "type": "organisations", "attributes": { "name": "Test Org" } },
                    { "id": "letter-3", "type": "letters", "attributes": { "status": "draft" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        var letters = result.Included!.OfType<Letter>().ToList();

        letters.Count.ShouldBe(2);
        letters[0].Id.ShouldBe("letter-2");
        letters[1].Id.ShouldBe("letter-3");
    }

    [Test]
    public void OfType_SameResourceUnderTwoDiscriminators_CollapsesToOne()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "sent" } },
                "included": [
                    { "id": "event-1", "type": "deliverables_events", "attributes": { "code": "transferred_to_distributor" } },
                    { "id": "event-1", "type": "letters_events", "attributes": { "code": "transferred_to_distributor" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        var events = result.Included!.OfType<LetterEvent>().ToList();

        events.ShouldSatisfyAllConditions(
            () => events.Count.ShouldBe(1),
            () => events[0].Id.ShouldBe("event-1"),
            () => events[0].Attributes.Code.ShouldBe("transferred_to_distributor"),
            () => PingenSerialisationHelper.TryGetIncludedData(result, out Data<LetterEvent>? single).ShouldBeTrue()
        );
    }

    [Test]
    public void OfType_DistinctResourcesUnderTwoDiscriminators_ReturnsBoth()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "sent" } },
                "included": [
                    { "id": "event-1", "type": "deliverables_events", "attributes": { "code": "transferred_to_distributor" } },
                    { "id": "event-2", "type": "letters_events", "attributes": { "code": "sent" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        var events = result.Included!.OfType<LetterEvent>().ToList();

        events.ShouldSatisfyAllConditions(() => events.Count.ShouldBe(2), () => events[0].Id.ShouldBe("event-1"), () => events[1].Id.ShouldBe("event-2"));
    }

    [Test]
    public void FindById_MatchingIdAndType_ReturnsItem()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "org-1", "type": "organisations", "attributes": { "name": "Org One" } },
                    { "id": "org-2", "type": "organisations", "attributes": { "name": "Org Two" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        Data<Organisation>? found = result.Included!.FindById<Organisation>("org-2");

        found.ShouldNotBeNull();
        found!.Id.ShouldBe("org-2");
        found.Attributes.Name.ShouldBe("Org Two");
    }

    [Test]
    public void FindById_NoMatchingId_ReturnsNull()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "org-1", "type": "organisations", "attributes": { "name": "Test Org" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        Data<Organisation>? found = result.Included!.FindById<Organisation>("org-999");

        found.ShouldBeNull();
    }

    [Test]
    public void FindById_MatchingIdWrongType_ReturnsNull()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "org-1", "type": "organisations", "attributes": { "name": "Test Org" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        Data<Letter>? found = result.Included!.FindById<Letter>("org-1");

        found.ShouldBeNull();
    }

    [Test]
    public void Count_ReflectsRawItemCount()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "org-1", "type": "organisations", "attributes": { "name": "Org One" } },
                    { "id": "org-2", "type": "organisations", "attributes": { "name": "Org Two" } },
                    { "id": "letter-2", "type": "letters", "attributes": { "status": "sent" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        result.Included!.Count.ShouldBe(3);
    }

    [Test]
    public void RawItems_IsReadOnly()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "org-1", "type": "organisations", "attributes": { "name": "Test Org" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        result.Included!.RawItems.ShouldBeAssignableTo<IReadOnlyList<JsonElement>>();
    }

    [Test]
    public void CollectionResult_IncludedCollection_WorksCorrectly()
    {
        string json = """
            {
                "data": [
                    { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } }
                ],
                "links": { "self": "https://example.com", "first": "https://example.com", "last": "https://example.com" },
                "meta": { "current_page": 1, "last_page": 1, "per_page": 100, "from": 1, "to": 1, "total": 1 },
                "included": [
                    { "id": "org-1", "type": "organisations", "attributes": { "name": "Test Org" } }
                ]
            }
            """;
        CollectionResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<CollectionResult<Data<Letter>>>(json)!;

        result.Included.ShouldNotBeNull();
        var orgs = result.Included!.OfType<Organisation>().ToList();
        orgs.Count.ShouldBe(1);
        orgs[0].Attributes.Name.ShouldBe("Test Org");
    }

    [Test]
    public void FindById_DuplicateEntries_ReturnsFirstMatch()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "org-1", "type": "organisations", "attributes": { "name": "First" } },
                    { "id": "org-1", "type": "organisations", "attributes": { "name": "Second" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        Data<Organisation>? found = result.Included!.FindById<Organisation>("org-1");

        found.ShouldNotBeNull();
        found!.Attributes.Name.ShouldBe("First");
    }

    [Test]
    public void OfType_DuplicateEntries_ReturnsAllInsertionOrder()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "org-1", "type": "organisations", "attributes": { "name": "First" } },
                    { "id": "org-1", "type": "organisations", "attributes": { "name": "Second" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        var orgs = result.Included!.OfType<Organisation>().ToList();

        orgs.ShouldSatisfyAllConditions(
            () => orgs.Count.ShouldBe(2),
            () => orgs[0].Attributes.Name.ShouldBe("First"),
            () => orgs[1].Attributes.Name.ShouldBe("Second")
        );
    }

    [Test]
    public void Constructor_AcceptsRawItemsList_StoresThem()
    {
        var emptyCollection = new IncludedCollection(new List<JsonElement>());

        JsonElement element = JsonDocument.Parse("{\"id\":\"x\",\"type\":\"letters\"}").RootElement;
        var populatedCollection = new IncludedCollection([element]);

        emptyCollection.Count.ShouldBe(0);
        populatedCollection.ShouldSatisfyAllConditions(
            () => populatedCollection.Count.ShouldBe(1),
            () => populatedCollection.RawItems[0].GetProperty("id").GetString().ShouldBe("x")
        );
    }

    [Test]
    public void OfType_ItemMissingTypeProperty_SkipsElement()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "x", "attributes": {} },
                    { "id": "org-1", "type": "organisations", "attributes": { "name": "Test" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        var orgs = result.Included!.OfType<Organisation>().ToList();

        orgs.Count.ShouldBe(1);
    }

    [Test]
    public void OfType_ItemWithNullTypeProperty_SkipsElement()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "x", "type": null, "attributes": {} },
                    { "id": "org-1", "type": "organisations", "attributes": { "name": "Test" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        var orgs = result.Included!.OfType<Organisation>().ToList();

        orgs.Count.ShouldBe(1);
    }

    [Test]
    public void FindById_ItemMissingIdProperty_SkipsElement()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "type": "organisations", "attributes": { "name": "X" } },
                    { "id": "org-2", "type": "organisations", "attributes": { "name": "Org Two" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        Data<Organisation>? found = result.Included!.FindById<Organisation>("org-2");

        found.ShouldNotBeNull();
        found!.Id.ShouldBe("org-2");
    }

    [Test]
    public void FindById_ItemMissingTypeProperty_SkipsElement()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "org-1", "attributes": {} },
                    { "id": "org-2", "type": "organisations", "attributes": { "name": "Org Two" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        Data<Organisation>? found = result.Included!.FindById<Organisation>("org-2");

        found.ShouldNotBeNull();
        found!.Id.ShouldBe("org-2");
    }

    [Test]
    public void FindById_ItemWithNullTypeProperty_SkipsElement()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "org-1", "type": null, "attributes": {} },
                    { "id": "org-2", "type": "organisations", "attributes": { "name": "Org Two" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        Data<Organisation>? found = result.Included!.FindById<Organisation>("org-2");

        found.ShouldNotBeNull();
        found!.Id.ShouldBe("org-2");
    }

    [Test]
    public void JsonConverter_RoundTrip_PreservesIncludedArray()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "org-1", "type": "organisations", "attributes": { "name": "Org One" } },
                    { "id": "org-2", "type": "organisations", "attributes": { "name": "Org Two" } }
                ]
            }
            """;

        SingleResult<Data<Letter>>? first = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;
        string serialized = PingenSerialisationHelper.Serialize(first);
        SingleResult<Data<Letter>>? second = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(serialized)!;

        var orgs = second.Included!.OfType<Organisation>().ToList();
        second.ShouldSatisfyAllConditions(
            () => second.Included!.Count.ShouldBe(2),
            () => orgs.Count.ShouldBe(2),
            () => orgs[0].Id.ShouldBe("org-1"),
            () => orgs[1].Id.ShouldBe("org-2")
        );
    }

    [Test]
    public void FindById_UserAssociationInIncludedArray_ReturnsItem()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "assoc-1", "type": "associations", "attributes": { "role": "owner", "status": "active" } },
                    { "id": "assoc-2", "type": "associations", "attributes": { "role": "manager", "status": "active" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        Data<UserAssociation>? found = result.Included!.FindById<UserAssociation>("assoc-2");

        found.ShouldNotBeNull();
        found!.ShouldSatisfyAllConditions(
            () => found.Id.ShouldBe("assoc-2"),
            () => found.Attributes.Role.ShouldBe("manager"),
            () => found.Attributes.Status.ShouldBe("active")
        );
    }

    [Test]
    public void FindById_HeterogeneousIncludedArray_ReturnsCorrectTypedItem()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "org-7", "type": "organisations", "attributes": { "name": "Acme Org" } },
                    { "id": "letter-9", "type": "letters", "attributes": { "status": "sent" } },
                    { "id": "assoc-3", "type": "associations", "attributes": { "role": "owner", "status": "pending" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        Data<Letter>? letter = result.Included!.FindById<Letter>("letter-9");
        Data<Organisation>? org = result.Included!.FindById<Organisation>("org-7");
        Data<UserAssociation>? assoc = result.Included!.FindById<UserAssociation>("assoc-3");

        letter.ShouldNotBeNull();
        org.ShouldNotBeNull();
        assoc.ShouldNotBeNull();
        letter!.Id.ShouldBe("letter-9");
        org!.Attributes.Name.ShouldBe("Acme Org");
        assoc!.Attributes.Role.ShouldBe("owner");
    }

    [Test]
    public void FindById_SameIdDifferentTypes_ResolvesByTypeDiscriminator()
    {
        string json = """
            {
                "data": { "id": "letter-1", "type": "letters", "attributes": { "status": "valid" } },
                "included": [
                    { "id": "shared-id", "type": "organisations", "attributes": { "name": "Org With Shared Id" } },
                    { "id": "shared-id", "type": "letters", "attributes": { "status": "sent" } }
                ]
            }
            """;
        SingleResult<Data<Letter>>? result = PingenSerialisationHelper.Deserialize<SingleResult<Data<Letter>>>(json)!;

        Data<Organisation>? org = result.Included!.FindById<Organisation>("shared-id");
        Data<Letter>? letter = result.Included!.FindById<Letter>("shared-id");

        org.ShouldNotBeNull();
        letter.ShouldNotBeNull();
        org!.Attributes.Name.ShouldBe("Org With Shared Id");
        letter!.Attributes.Status.ShouldBe("sent");
    }
}

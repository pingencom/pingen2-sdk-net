using System.Text.Json;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Batches;
using Pingen.Abstractions.Models.Letters;
using Pingen.Abstractions.Models.Organisations;
using Pingen.Tests.Helpers;

namespace Pingen.Tests.CrossCutting;

[TestFixture]
public sealed class EdgeCaseTests : EndpointTestBase
{
    [Test]
    public async Task GetPage_EmptyCollection_ShouldReturnEmptyList()
    {
        Server.StubJsonGet(OrgPath("batches"), PingenResponseFactory.BatchCollection(0));

        ApiResult<CollectionResult<BatchData>> result = await Client.Batches.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.ShouldNotBeNull(),
            () => result.Data!.Data.ShouldBeEmpty()
        );
    }

    [Test]
    public async Task Get_WithAllNullOptionalFields_ShouldDeserializeSuccessfully()
    {
        string batchId = Guid.NewGuid().ToString();
        string body = JsonSerializer.Serialize(
            new
            {
                data = new
                {
                    id = batchId,
                    type = "batches",
                    attributes = new
                    {
                        name = (string?)null,
                        icon = (string?)null,
                        status = (string?)null,
                        file_original_name = (string?)null,
                        letter_count = (int?)null,
                        address_position = (string?)null,
                        print_mode = (string?)null,
                        print_spectrum = (string?)null,
                        price_currency = (string?)null,
                        price_value = (double?)null,
                        submitted_at = (string?)null,
                        created_at = (string?)null,
                        updated_at = (string?)null,
                    },
                    relationships = new
                    {
                        organisation = JsonApiStubHelper.RelatedSingle(TestOrganisationId, "organisations"),
                        events = JsonApiStubHelper.RelatedMany(),
                    },
                    meta = JsonApiStubHelper.MetaWithAbilities(
                        new
                        {
                            cancel = "ok",
                            delete = "ok",
                            submit = "ok",
                            edit = "ok",
                            change_window_position = "ok",
                            add_attachment = "ok",
                        }
                    ),
                },
            }
        );

        Server.StubJsonGet(OrgPath($"batches/{batchId}"), body);

        ApiResult<SingleResult<BatchDataDetailed>> result = await Client.Batches.Get(batchId);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Id.ShouldBe(batchId),
            () => result.Data!.Data.Attributes.Name.ShouldBeNull(),
            () => result.Data!.Data.Attributes.Status.ShouldBeNull(),
            () => result.Data!.Data.Attributes.LetterCount.ShouldBeNull()
        );
    }

    [Test]
    public async Task GetPage_WithIncludedArray_ShouldDeserializeIncluded()
    {
        string orgId = TestOrganisationId;
        string letterId = Guid.NewGuid().ToString();

        string body = JsonSerializer.Serialize(
            new
            {
                data = new[]
                {
                    new
                    {
                        id = letterId,
                        type = "letters",
                        attributes = new
                        {
                            status = "valid",
                            file_original_name = "test.pdf",
                            file_pages = 1,
                        },
                        relationships = new
                        {
                            organisation = JsonApiStubHelper.RelatedSingle(orgId, "organisations"),
                            events = JsonApiStubHelper.RelatedMany(),
                        },
                    },
                },
                included = new object[]
                {
                    new
                    {
                        id = orgId,
                        type = "organisations",
                        attributes = new
                        {
                            name = "Included Org",
                            status = "active",
                            plan = "professional",
                            billing_mode = "prepaid",
                            billing_currency = "CHF",
                            billing_balance = 100.0,
                            default_country = "CH",
                        },
                    },
                },
                links = new
                {
                    first = "https://api.test.pingen.com/?page[number]=1",
                    last = "https://api.test.pingen.com/?page[number]=1",
                    prev = (string?)null,
                    next = (string?)null,
                    self = "https://api.test.pingen.com/?page[number]=1",
                },
                meta = new
                {
                    current_page = 1,
                    last_page = 1,
                    per_page = 20,
                    from = 1,
                    to = 1,
                    total = 1,
                },
            }
        );

        Server.StubJsonGet(OrgPath("deliveries/letters"), body);

        ApiResult<CollectionResult<LetterData>> result = await Client.Letters.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Included.ShouldNotBeNull(),
            () => result.Data!.Included!.Count.ShouldBe(1)
        );

        var includedOrgs = result.Data!.Included!.OfType<Organisation>().ToList();
        includedOrgs.Count.ShouldBe(1);
        includedOrgs[0].Id.ShouldBe(orgId);
        includedOrgs[0].Attributes.Name.ShouldBe("Included Org");
    }

    [Test]
    public async Task GetPage_With100Items_ShouldDeserializeAll()
    {
        const int itemCount = 100;

        Server.StubJsonGet(OrgPath("batches"), PingenResponseFactory.BatchCollection(itemCount));

        ApiResult<CollectionResult<BatchData>> result = await Client.Batches.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Count.ShouldBe(itemCount)
        );

        result.Data!.Data.Select(d => d.Id).Distinct().Count().ShouldBe(itemCount);
    }

    [Test]
    public async Task GetPage_WithoutIncludedKey_ShouldHaveNullIncluded()
    {
        Server.StubJsonGet(OrgPath("batches"), PingenResponseFactory.BatchCollection(2));

        ApiResult<CollectionResult<BatchData>> result = await Client.Batches.GetPage();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Included.ShouldBeNull()
        );
    }
}

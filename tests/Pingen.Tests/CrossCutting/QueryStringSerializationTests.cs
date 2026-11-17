using System.Web;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Letters;
using Pingen.Tests.Helpers;
using WireMock.Logging;
using WireMock.Types;

namespace Pingen.Tests.CrossCutting;

[TestFixture]
public sealed class QueryStringSerializationTests : EndpointTestBase
{
    [Test]
    public async Task GetPage_WithNestedAndOrFilter_ShouldSerializeAsUrlEncodedJson()
    {
        Server.StubJsonGet(OrgPath("deliveries/letters"), PingenResponseFactory.LetterCollection(0));

        var apiPagingRequest = new ApiPagingRequest
        {
            Filtering = new KeyValuePair<string, object>(
                CollectionFilterOperator.And,
                new KeyValuePair<string, object>[]
                {
                    new(CollectionFilterOperator.Or, new KeyValuePair<string, object>[] { new(LetterFields.Country, "CH"), new(LetterFields.Country, "DE") }),
                    new(LetterFields.Status, "valid"),
                }
            ),
        };

        await Client.Letters.GetPage(apiPagingRequest);

        ILogEntry entry = Server.LogEntries.Single(e => e.RequestMessage?.Path == OrgPath("deliveries/letters"));
        IDictionary<string, WireMockList<string>>? query = entry.RequestMessage!.Query!;
        query.ShouldContainKey("filter");

        string filterValue = query["filter"][0];
        filterValue.ShouldBe("""{"and":[{"or":[{"country":"CH"},{"country":"DE"}]},{"status":"valid"}]}""");

        string rawQuery = entry.RequestMessage!.RawQuery!;
        rawQuery.ShouldContain("filter=");
        HttpUtility.UrlDecode(rawQuery).ShouldContain(filterValue);
    }

    [Test]
    public async Task GetPage_WithMultiFieldSort_ShouldSerializeAsCommaSeparatedList()
    {
        Server.StubJsonGet(OrgPath("deliveries/letters"), PingenResponseFactory.LetterCollection(0));

        var apiPagingRequest = new ApiPagingRequest
        {
            Sorting = new[]
            {
                new KeyValuePair<string, CollectionSortDirection>(LetterFields.CreatedAt, CollectionSortDirection.ASC),
                new KeyValuePair<string, CollectionSortDirection>(LetterFields.Status, CollectionSortDirection.DESC),
            },
        };

        await Client.Letters.GetPage(apiPagingRequest);

        ILogEntry entry = Server.LogEntries.Single(e => e.RequestMessage?.Path == OrgPath("deliveries/letters"));
        IDictionary<string, WireMockList<string>>? query = entry.RequestMessage!.Query!;
        query.ShouldContainKey("sort");
        query["sort"][0].ShouldBe("created_at,-status");
    }

    [Test]
    public async Task GetPage_WithSearchString_ShouldSerializeAsQParameter()
    {
        Server.StubJsonGet(OrgPath("deliveries/letters"), PingenResponseFactory.LetterCollection(0));

        const string searchTerm = "important-customer";
        var apiPagingRequest = new ApiPagingRequest { Searching = searchTerm };

        await Client.Letters.GetPage(apiPagingRequest);

        ILogEntry entry = Server.LogEntries.Single(e => e.RequestMessage?.Path == OrgPath("deliveries/letters"));
        IDictionary<string, WireMockList<string>>? query = entry.RequestMessage!.Query!;
        query.ShouldContainKey("q");
        query["q"][0].ShouldBe(searchTerm);
    }

    [Test]
    public async Task GetPage_WithPageNumberAndLimit_ShouldSerializePageParameters()
    {
        Server.StubJsonGet(OrgPath("deliveries/letters"), PingenResponseFactory.LetterCollection(0));

        var apiPagingRequest = new ApiPagingRequest { PageNumber = 3, PageLimit = 50 };

        await Client.Letters.GetPage(apiPagingRequest);

        ILogEntry entry = Server.LogEntries.Single(e => e.RequestMessage?.Path == OrgPath("deliveries/letters"));
        string decodedQuery = HttpUtility.UrlDecode(entry.RequestMessage!.RawQuery!);

        decodedQuery.ShouldSatisfyAllConditions(() => decodedQuery.ShouldContain("page[number]=3"), () => decodedQuery.ShouldContain("page[limit]=50"));
    }

    [Test]
    public async Task Get_WithSparseFieldsetsAndInclude_ShouldSerializeBothParameters()
    {
        string letterId = Guid.NewGuid().ToString();
        Server.StubJsonGet(OrgPath($"deliveries/letters/{letterId}"), PingenResponseFactory.SingleLetter(letterId));

        var apiRequest = new ApiRequest
        {
            SparseFieldsets = new[] { new KeyValuePair<string, IEnumerable<string>>("letters", new[] { LetterFields.Status, LetterFields.FileOriginalName }) },
            Include = new[] { LetterIncludes.Organisation, LetterIncludes.Batch },
        };

        await Client.Letters.Get(letterId, apiRequest);

        ILogEntry entry = Server.LogEntries.Single(e => e.RequestMessage?.Path == OrgPath($"deliveries/letters/{letterId}"));
        string decodedQuery = HttpUtility.UrlDecode(entry.RequestMessage!.RawQuery!);

        decodedQuery.ShouldSatisfyAllConditions(
            () => decodedQuery.ShouldContain("fields[letters]=status,file_original_name"),
            () => decodedQuery.ShouldContain("include=organisation,batch")
        );
    }
}

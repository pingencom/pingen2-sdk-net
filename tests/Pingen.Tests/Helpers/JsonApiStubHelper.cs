using System.Text.Json;

namespace Pingen.Tests.Helpers;

internal static class JsonApiStubHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

    internal static string TokenResponse(string token = "test-access-token", long expiresIn = 3600)
    {
        return JsonSerializer.Serialize(
            new
            {
                access_token = token,
                token_type = "Bearer",
                expires_in = expiresIn,
            },
            JsonOptions
        );
    }

    internal static string SingleResponse(string id, string type, object attributes, object? relationships = null, object? meta = null)
    {
        var data = BuildDataObject(id, type, attributes, relationships, meta);
        return JsonSerializer.Serialize(new { data }, JsonOptions);
    }

    internal static string CollectionResponse(
        IEnumerable<(string Id, object Attributes, object? Relationships, object? Meta)> items,
        string type,
        int currentPage = 1,
        int lastPage = 1,
        int perPage = 20,
        int? total = null
    )
    {
        var itemList = items.ToList();
        var actualTotal = total ?? itemList.Count;

        var data = itemList.Select(item => BuildDataObject(item.Id, type, item.Attributes, item.Relationships, item.Meta)).ToList();

        return JsonSerializer.Serialize(
            new
            {
                data,
                links = new
                {
                    first = "https://api.test.pingen.com/?page[number]=1",
                    last = $"https://api.test.pingen.com/?page[number]={lastPage}",
                    prev = (string?)null,
                    next = currentPage < lastPage ? $"https://api.test.pingen.com/?page[number]={currentPage + 1}" : null,
                    self = $"https://api.test.pingen.com/?page[number]={currentPage}",
                },
                meta = new
                {
                    current_page = currentPage,
                    last_page = lastPage,
                    per_page = perPage,
                    from = itemList.Count > 0 ? (currentPage - 1) * perPage + 1 : 0,
                    to = itemList.Count > 0 ? (currentPage - 1) * perPage + itemList.Count : 0,
                    total = actualTotal,
                },
            },
            JsonOptions
        );
    }

    private static Dictionary<string, object?> BuildDataObject(string id, string type, object attributes, object? relationships, object? meta)
    {
        var obj = new Dictionary<string, object?>
        {
            ["id"] = id,
            ["type"] = type,
            ["attributes"] = attributes,
        };

        if (relationships is not null)
            obj["relationships"] = relationships;

        if (meta is not null)
            obj["meta"] = meta;

        return obj;
    }

    internal static object RelatedSingle(string relatedId, string relatedType, string relatedUrl = "https://api.test.pingen.com/related")
    {
        return new { links = new { related = relatedUrl }, data = new { id = relatedId, type = relatedType } };
    }

    internal static object RelatedMany(
        string relatedUrl = "https://api.test.pingen.com/related",
        string href = "https://api.test.pingen.com/related",
        int metaCount = 0
    )
    {
        return new { links = new { related = new { href, meta = new { count = metaCount } } } };
    }

    internal static object MetaWithAbilities(object abilities)
    {
        return new { abilities = new { self = abilities } };
    }

    internal static object MetaWithOrganisationAbilities(object selfAbilities, object organisationAbilities)
    {
        return new { abilities = new { self = selfAbilities, organisation = organisationAbilities } };
    }
}

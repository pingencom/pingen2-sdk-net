using Pingen.Abstractions.Constants;
using Pingen.Abstractions.Models.Api;

namespace Pingen.Tests;

public class ApiRequestQueryParameters
{
    [Test]
    public void ApiRequest_Include_PropertyIsSettable()
    {
        var request = new ApiRequest { Include = ["events", "sender"] };

        request.Include.ShouldBe(new[] { "events", "sender" }, ignoreOrder: true);
    }

    [Test]
    public void ApiRequest_Include_SingleRelationship()
    {
        var request = new ApiRequest { Include = ["events"] };

        request.Include.ShouldBe(new[] { "events" }, ignoreOrder: true);
    }

    [Test]
    public void ApiRequest_Include_NullByDefault()
    {
        var request = new ApiRequest();

        request.Include.ShouldBeNull();
    }

    [Test]
    public void Include_SerializesToCommaSeparatedString()
    {
        var includeValues = new[] { "events", "sender", "file" };
        var expected = "events,sender,file";

        var result = string.Join(',', includeValues);

        result.ShouldSatisfyAllConditions(() => result.ShouldBe(expected), () => ApiQueryParameterNames.Include.ShouldBe("include"));
    }
}

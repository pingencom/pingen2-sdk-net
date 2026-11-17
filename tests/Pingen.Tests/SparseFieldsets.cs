using System.Reflection;
using Pingen.Abstractions.Constants;
using Pingen.Abstractions.Models.Api;

namespace Pingen.Tests;

public class SparseFieldsets
{
    [Test]
    public void ApiRequest_HasSparseFieldsetsProperty()
    {
        var request = new ApiRequest { SparseFieldsets = new[] { new KeyValuePair<string, IEnumerable<string>>("letters", new[] { "name", "status" }) } };

        request.SparseFieldsets.ShouldNotBeNull();
        request.SparseFieldsets!.Count().ShouldBe(1);
    }

    [Test]
    public void ApiRequest_SparseFieldsetsDefaultsToNull()
    {
        var request = new ApiRequest();
        request.SparseFieldsets.ShouldBeNull();
    }

    [Test]
    public void ApiPagingRequest_InheritsSparseFieldsetsFromApiRequest()
    {
        var request = new ApiPagingRequest
        {
            SparseFieldsets = new[]
            {
                new KeyValuePair<string, IEnumerable<string>>("letters", new[] { "name", "status" }),
                new KeyValuePair<string, IEnumerable<string>>("organisations", new[] { "name" }),
            },
            PageLimit = 10,
        };

        request.ShouldSatisfyAllConditions(
            () => request.SparseFieldsets.ShouldNotBeNull(),
            () => request.SparseFieldsets!.Count().ShouldBe(2),
            () => request.PageLimit.ShouldBe(10)
        );
    }

    [Test]
    public void ApiQueryParameterNames_SparseFields_GeneratesCorrectFormat()
    {
        "ApiQueryParameterNames".ShouldSatisfyAllConditions(
            () => ApiQueryParameterNames.SparseFields("letters").ShouldBe("fields[letters]"),
            () => ApiQueryParameterNames.SparseFields("organisations").ShouldBe("fields[organisations]"),
            () => ApiQueryParameterNames.SparseFields("webhooks").ShouldBe("fields[webhooks]"),
            () => ApiQueryParameterNames.SparseFields("batches").ShouldBe("fields[batches]"),
            () => ApiQueryParameterNames.SparseFields("users").ShouldBe("fields[users]")
        );
    }

    [Test]
    public void GetQueryParameters_SerializesSparseFieldsets()
    {
        var apiRequest = new ApiRequest { SparseFieldsets = new[] { new KeyValuePair<string, IEnumerable<string>>("letters", new[] { "name", "status" }) } };

        var result = InvokeGetQueryParameters(apiRequest);

        result.ShouldNotBeNull();
        var parameters = result!.ToList();
        parameters.Count.ShouldBe(1);
        parameters.ShouldSatisfyAllConditions(() => parameters[0].Key.ShouldBe("fields[letters]"), () => parameters[0].Value.ShouldBe("name,status"));
    }

    [Test]
    public void GetQueryParameters_SerializesMultipleSparseFieldsets()
    {
        var apiRequest = new ApiRequest
        {
            SparseFieldsets = new[]
            {
                new KeyValuePair<string, IEnumerable<string>>("letters", new[] { "name", "status" }),
                new KeyValuePair<string, IEnumerable<string>>("organisations", new[] { "name" }),
            },
        };

        var result = InvokeGetQueryParameters(apiRequest);

        result.ShouldNotBeNull();
        var parameters = result!.ToList();
        parameters.Count.ShouldBe(2);
        parameters.ShouldSatisfyAllConditions(
            () => parameters[0].Key.ShouldBe("fields[letters]"),
            () => parameters[0].Value.ShouldBe("name,status"),
            () => parameters[1].Key.ShouldBe("fields[organisations]"),
            () => parameters[1].Value.ShouldBe("name")
        );
    }

    [Test]
    public void GetQueryParameters_HandlesNullSparseFieldsets()
    {
        var apiRequest = new ApiRequest();

        var result = InvokeGetQueryParameters(apiRequest);

        result.ShouldNotBeNull();
        var parameters = result!.ToList();
        parameters.Count.ShouldBe(0);
    }

    [Test]
    public void GetQueryParameters_HandlesEmptySparseFieldsets()
    {
        var apiRequest = new ApiRequest { SparseFieldsets = [] };

        var result = InvokeGetQueryParameters(apiRequest);

        result.ShouldNotBeNull();
        var parameters = result!.ToList();
        parameters.Count.ShouldBe(0);
    }

    [Test]
    public void GetQueryParameters_SparseFieldsetsWithPagingRequest()
    {
        var apiPagingRequest = new ApiPagingRequest
        {
            SparseFieldsets = new[] { new KeyValuePair<string, IEnumerable<string>>("letters", new[] { "name" }) },
            PageLimit = 10,
            PageNumber = 1,
        };

        var result = InvokeGetQueryParameters(apiPagingRequest);

        result.ShouldNotBeNull();
        var parameters = result!.ToList();
        parameters.Count.ShouldBeGreaterThanOrEqualTo(3);

        var fieldsParam = parameters.First(p => p.Key.StartsWith("fields["));
        fieldsParam.ShouldSatisfyAllConditions(() => fieldsParam.Key.ShouldBe("fields[letters]"), () => fieldsParam.Value.ShouldBe("name"));
    }

    [Test]
    public void IPingenConnectionHandler_HasGetAsyncOverloadsWithApiRequest()
    {
        var interfaceType = typeof(IPingenConnectionHandler);

        // GetAsync<TResult>(string, ApiRequest?, CancellationToken) should exist
        var genericMethod = interfaceType
            .GetMethods()
            .Where(m => m.Name == "GetAsync" && m.IsGenericMethod)
            .FirstOrDefault(m =>
            {
                var parameters = m.GetParameters();
                return parameters.Length == 3 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(ApiRequest);
            });
        genericMethod.ShouldNotBeNull("GetAsync<TResult>(string, ApiRequest?, CancellationToken) should exist on IPingenConnectionHandler");

        // GetAsync(string, ApiRequest?, CancellationToken) should exist
        var nonGenericMethod = interfaceType
            .GetMethods()
            .Where(m => m.Name == "GetAsync" && !m.IsGenericMethod)
            .FirstOrDefault(m =>
            {
                var parameters = m.GetParameters();
                return parameters.Length == 3 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(ApiRequest);
            });
        nonGenericMethod.ShouldNotBeNull("GetAsync(string, ApiRequest?, CancellationToken) should exist on IPingenConnectionHandler");
    }

    [Test]
    public void ConnectorServiceInterfaces_GetMethodsAcceptApiRequest()
    {
        // ILetterService.Get should have overload with ApiRequest
        var letterGet = typeof(Pingen.Interfaces.Connectors.ILetterService).GetMethod("Get", [typeof(string), typeof(ApiRequest), typeof(CancellationToken)]);
        letterGet.ShouldNotBeNull("ILetterService.Get should accept ApiRequest");

        // IBatchService.Get should have overload with ApiRequest
        var batchGet = typeof(Pingen.Interfaces.Connectors.IBatchService).GetMethod("Get", [typeof(string), typeof(ApiRequest), typeof(CancellationToken)]);
        batchGet.ShouldNotBeNull("IBatchService.Get should accept ApiRequest");

        // IOrganisationService.Get should have overload with ApiRequest
        var orgGet = typeof(Pingen.Interfaces.Connectors.IOrganisationService).GetMethod(
            "Get",
            [typeof(string), typeof(ApiRequest), typeof(CancellationToken)]
        );
        orgGet.ShouldNotBeNull("IOrganisationService.Get should accept ApiRequest");

        // IWebhookService.Get should have overload with ApiRequest
        var webhookGet = typeof(Pingen.Interfaces.Connectors.IWebhookService).GetMethod("Get", [typeof(string), typeof(ApiRequest), typeof(CancellationToken)]);
        webhookGet.ShouldNotBeNull("IWebhookService.Get should accept ApiRequest");

        // IUserService.Get should have overload with ApiRequest
        var userGet = typeof(Pingen.Interfaces.Connectors.IUserService).GetMethod("Get", [typeof(ApiRequest), typeof(CancellationToken)]);
        userGet.ShouldNotBeNull("IUserService.Get should accept ApiRequest");
    }

    private static IEnumerable<KeyValuePair<string, string>>? InvokeGetQueryParameters(ApiRequest? apiRequest)
    {
        var method = typeof(PingenConnectionHandler).GetMethod("GetQueryParameters", BindingFlags.NonPublic | BindingFlags.Static, [typeof(ApiRequest)]);

        return method?.Invoke(null, [apiRequest]) as IEnumerable<KeyValuePair<string, string>>;
    }
}

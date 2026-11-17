using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Pingen.Tests.Helpers;

internal static class WireMockExtensions
{
    internal static void StubJsonGet(this WireMockServer server, string path, string body, int statusCode = 200)
    {
        server
            .Given(Request.Create().WithPath(path).UsingGet())
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(statusCode)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithBody(body)
            );
    }

    internal static void StubJsonPost(this WireMockServer server, string path, string body, int statusCode = 201)
    {
        server
            .Given(Request.Create().WithPath(path).UsingPost())
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(statusCode)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithBody(body)
            );
    }

    internal static void StubJsonPatch(this WireMockServer server, string path, string body, int statusCode = 200)
    {
        server
            .Given(Request.Create().WithPath(path).UsingPatch())
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(statusCode)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
                    .WithBody(body)
            );
    }

    internal static void StubDelete(this WireMockServer server, string path, int statusCode = 204)
    {
        server
            .Given(Request.Create().WithPath(path).UsingDelete())
            .RespondWith(Response.Create().WithStatusCode(statusCode).WithHeader("X-Request-ID", Guid.NewGuid().ToString()));
    }

    internal static void StubError(this WireMockServer server, string path, string method, string body, int statusCode = 422)
    {
        server
            .Given(Request.Create().WithPath(path).UsingMethod(method))
            .RespondWith(Response.Create().WithStatusCode(statusCode).WithHeader("Content-Type", "application/json").WithBody(body));
    }
}

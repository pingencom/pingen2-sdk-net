using System.Net;
using Pingen.Abstractions.Constants;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Exceptions;
using Pingen.Abstractions.Helpers;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Base;
using Pingen.Abstractions.Models.Webhooks;
using Pingen.Abstractions.Models.Webhooks.Views;
using Pingen.Tests.Helpers;

namespace Pingen.Tests.Client;

[NonParallelizable]
public class PingenConnectionHandlerTests
{
    [Test]
    public void Constructor_NullBaseUri_ThrowsArgumentException()
    {
        var config = CreateConfig(baseUri: "", identityUri: "https://identity.example.com/");

        Should.Throw<ArgumentException>(() => new PingenConnectionHandler(config, CreateHttpClients()));
    }

    [Test]
    public void Constructor_NullIdentityUri_ThrowsArgumentException()
    {
        var config = CreateConfig(baseUri: "https://api.example.com/", identityUri: "");

        Should.Throw<ArgumentException>(() => new PingenConnectionHandler(config, CreateHttpClients()));
    }

    [Test]
    public void Constructor_ValidConfig_Succeeds()
    {
        var config = CreateConfig();

        var handler = new PingenConnectionHandler(config, CreateHttpClients());

        handler.ShouldNotBeNull();
    }

    [Test]
    public void SetOrganisationId_UpdatesOrganisationId()
    {
        var config = CreateConfig();
        var handler = new PingenConnectionHandler(config, CreateHttpClients());

        // Should not throw
        handler.SetOrganisationId("new-org-id");
    }

    [Test]
    public async Task GetAsync_AuthenticatesAndSendsRequest()
    {
        var tokenJson = PingenSerialisationHelper.Serialize(
            new
            {
                access_token = "test-token",
                token_type = "Bearer",
                expires_in = 3600,
            }
        );

        var identityHandler = new MockHttpMessageHandler(HttpStatusCode.OK, tokenJson);
        var apiHandler = new MockHttpMessageHandler(
            (request, _) =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":[]}") };
                return Task.FromResult(response);
            }
        );

        var config = CreateConfig();
        var httpClients = CreateHttpClients(identityHandler, apiHandler);
        var handler = new PingenConnectionHandler(config, httpClients);

        var result = await handler.GetAsync("letters", (ApiPagingRequest?)null);

        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task GetAsync_WhenOrganisationIdNotSet_ThrowsInvalidOperationException()
    {
        var tokenJson = PingenSerialisationHelper.Serialize(
            new
            {
                access_token = "test-token",
                token_type = "Bearer",
                expires_in = 3600,
            }
        );

        var identityHandler = new MockHttpMessageHandler(HttpStatusCode.OK, tokenJson);
        var apiHandler = new MockHttpMessageHandler(HttpStatusCode.OK, "{\"data\":[]}");

        var config = CreateConfig(defaultOrganisationId: "");
        var handler = new PingenConnectionHandler(config, CreateHttpClients(identityHandler, apiHandler));

        await Should.ThrowAsync<InvalidOperationException>(async () => await handler.GetAsync("letters", (ApiPagingRequest?)null));
    }

    [Test]
    public async Task SendExternalRequestAsync_SendsRequest()
    {
        var externalHandler = new MockHttpMessageHandler(HttpStatusCode.OK, "file-content");

        var config = CreateConfig();
        var httpClients = CreateHttpClients(externalHandler: externalHandler);
        var handler = new PingenConnectionHandler(config, httpClients);

        var request = new HttpRequestMessage(HttpMethod.Get, "https://external.example.com/file.pdf");
        var result = await handler.SendExternalRequestAsync(request);

        result.IsSuccessStatusCode.ShouldBeTrue();
    }

    [Test]
    public async Task MultipleInstances_MaintainSeparateAccessTokens()
    {
        // Handler A identity server returns token-A
        var identityHandlerA = new MockHttpMessageHandler(
            HttpStatusCode.OK,
            PingenSerialisationHelper.Serialize(
                new
                {
                    access_token = "token-A",
                    token_type = "Bearer",
                    expires_in = 3600,
                }
            )
        );

        string? capturedTokenA = null;
        var apiHandlerA = new MockHttpMessageHandler(
            (request, _) =>
            {
                capturedTokenA = request.Headers.Authorization?.Parameter;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":[]}") });
            }
        );

        // Handler B identity server returns token-B
        var identityHandlerB = new MockHttpMessageHandler(
            HttpStatusCode.OK,
            PingenSerialisationHelper.Serialize(
                new
                {
                    access_token = "token-B",
                    token_type = "Bearer",
                    expires_in = 3600,
                }
            )
        );

        string? capturedTokenB = null;
        var apiHandlerB = new MockHttpMessageHandler(
            (request, _) =>
            {
                capturedTokenB = request.Headers.Authorization?.Parameter;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":[]}") });
            }
        );

        var configA = CreateConfig();
        var configB = CreateConfig();

        var httpClientsA = CreateHttpClients(identityHandlerA, apiHandlerA);
        var httpClientsB = CreateHttpClients(identityHandlerB, apiHandlerB);

        // Handler A authenticates first
        var handlerA = new PingenConnectionHandler(configA, httpClientsA);
        await handlerA.GetAsync("letters", (ApiPagingRequest?)null);

        // Handler B created after A has already authenticated - must use its own token
        var handlerB = new PingenConnectionHandler(configB, httpClientsB);
        await handlerB.GetAsync("letters", (ApiPagingRequest?)null);

        // Each handler should use its own token, not share the static one
        capturedTokenA.ShouldBe("token-A");
        capturedTokenB.ShouldBe("token-B");
    }

    [Test]
    public async Task GetAsync_NonSuccessStatusCode_ReturnsFailureResult()
    {
        var tokenJson = PingenSerialisationHelper.Serialize(
            new
            {
                access_token = "test-token",
                token_type = "Bearer",
                expires_in = 3600,
            }
        );

        var identityHandler = new MockHttpMessageHandler(HttpStatusCode.OK, tokenJson);
        var apiHandler = new MockHttpMessageHandler(
            HttpStatusCode.BadRequest,
            "{\"errors\":[{\"code\":\"400\",\"title\":\"Bad Request\",\"detail\":\"Invalid request\",\"source\":{\"pointer\":\"/data\",\"parameter\":\"\"}}]}"
        );

        var config = CreateConfig();
        var httpClients = CreateHttpClients(identityHandler, apiHandler);
        var handler = new PingenConnectionHandler(config, httpClients);

        var result = await handler.GetAsync("letters", (ApiPagingRequest?)null);

        result.IsSuccess.ShouldBeFalse();
    }

    [Test]
    public async Task SetOrUpdateAccessToken_ConcurrentCalls_OnlyAuthenticatesOnce()
    {
        var loginCallCount = 0;
        var loginEnteredEvent = new ManualResetEventSlim(false);

        var identityHandler = new MockHttpMessageHandler(
            (_, _) =>
            {
                Interlocked.Increment(ref loginCallCount);
                loginEnteredEvent.Set();

                var tokenJson = PingenSerialisationHelper.Serialize(
                    new
                    {
                        access_token = "shared-token",
                        token_type = "Bearer",
                        expires_in = 3600,
                    }
                );

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(tokenJson) });
            }
        );

        var apiHandler = new MockHttpMessageHandler(
            (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":[]}") })
        );

        var config = CreateConfig();
        var httpClients = CreateHttpClients(identityHandler, apiHandler);
        var handler = new PingenConnectionHandler(config, httpClients);

        // Fire two concurrent requests that both need authentication
        var task1 = handler.GetAsync("letters", (ApiPagingRequest?)null);
        var task2 = handler.GetAsync("letters", (ApiPagingRequest?)null);
        await Task.WhenAll(task1, task2);

        // Both requests should succeed
        task1.Result.IsSuccess.ShouldBeTrue();
        task2.Result.IsSuccess.ShouldBeTrue();

        // Only one login call should have been made
        loginCallCount.ShouldBe(1);
    }

    [Test]
    public async Task GetAsync_WithRateLimitHeaders_ParsesHeadersIntoApiResult()
    {
        var tokenJson = PingenSerialisationHelper.Serialize(
            new
            {
                access_token = "test-token",
                token_type = "Bearer",
                expires_in = 3600,
            }
        );

        var requestId = Guid.NewGuid();
        var resetUnixTimestamp = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds();

        var identityHandler = new MockHttpMessageHandler(HttpStatusCode.OK, tokenJson);
        var apiHandler = new MockHttpMessageHandler(
            (_, _) =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":[]}") };
                response.Headers.Add("X-Request-ID", requestId.ToString());
                response.Headers.Add("x-ratelimit-limit", "100");
                response.Headers.Add("x-ratelimit-remaining", "42");
                response.Headers.Add("x-ratelimit-reset", resetUnixTimestamp.ToString());
                return Task.FromResult(response);
            }
        );

        var config = CreateConfig();
        var httpClients = CreateHttpClients(identityHandler, apiHandler);
        var handler = new PingenConnectionHandler(config, httpClients);

        var result = await handler.GetAsync("letters", (ApiPagingRequest?)null);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.RequestId.ShouldBe(requestId),
            () => result.RateLimitLimit.ShouldBe(100),
            () => result.RateLimitRemaining.ShouldBe(42),
            () => result.RateLimitReset.ShouldBe(DateTimeOffset.FromUnixTimeSeconds(resetUnixTimestamp))
        );
    }

    [Test]
    public async Task GetAsync_WithMissingRateLimitHeaders_UsesDefaultValues()
    {
        var tokenJson = PingenSerialisationHelper.Serialize(
            new
            {
                access_token = "test-token",
                token_type = "Bearer",
                expires_in = 3600,
            }
        );

        var identityHandler = new MockHttpMessageHandler(HttpStatusCode.OK, tokenJson);
        var apiHandler = new MockHttpMessageHandler(
            (_, _) =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":[]}") };
                return Task.FromResult(response);
            }
        );

        var config = CreateConfig();
        var httpClients = CreateHttpClients(identityHandler, apiHandler);
        var handler = new PingenConnectionHandler(config, httpClients);

        var result = await handler.GetAsync("letters", (ApiPagingRequest?)null);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.RateLimitLimit.ShouldBe(0),
            () => result.RateLimitRemaining.ShouldBe(0),
            () => result.RateLimitReset.ShouldBeNull(),
            () => result.RetryAfter.ShouldBeNull()
        );
    }

    [Test]
    public async Task GetAsync_WithRetryAfterHeader_ParsesRetryAfter()
    {
        var tokenJson = PingenSerialisationHelper.Serialize(
            new
            {
                access_token = "test-token",
                token_type = "Bearer",
                expires_in = 3600,
            }
        );

        var identityHandler = new MockHttpMessageHandler(HttpStatusCode.OK, tokenJson);
        var apiHandler = new MockHttpMessageHandler(
            (_, _) =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":[]}") };
                response.Headers.TryAddWithoutValidation("Retry-After", "60");
                return Task.FromResult(response);
            }
        );

        var config = CreateConfig();
        var httpClients = CreateHttpClients(identityHandler, apiHandler);
        var handler = new PingenConnectionHandler(config, httpClients);

        var result = await handler.GetAsync("letters", (ApiPagingRequest?)null);

        result.RetryAfter.ShouldBe(60);
    }

    [Test]
    public async Task GetAsync_AuthenticationFailure_ThrowsPingenApiAuthErrorException()
    {
        var errorJson = PingenSerialisationHelper.Serialize(
            new
            {
                error = "invalid_client",
                error_description = "Client authentication failed",
                message = "Client authentication failed",
            }
        );

        var identityHandler = new MockHttpMessageHandler(HttpStatusCode.Unauthorized, errorJson);
        var apiHandler = new MockHttpMessageHandler(HttpStatusCode.OK, "{}");

        var config = CreateConfig();
        var httpClients = CreateHttpClients(identityHandler, apiHandler);
        var handler = new PingenConnectionHandler(config, httpClients);

        var ex = await Should.ThrowAsync<PingenApiAuthErrorException>(async () => await handler.GetAsync("letters", (ApiPagingRequest?)null));
        ex.ShouldSatisfyAllConditions(
            () => ex.Message.ShouldContain("Client authentication failed"),
            () => ex.AuthError.ShouldNotBeNull(),
            () => ex.AuthError!.Error.ShouldBe("invalid_client"),
            () => ex.AuthError!.ErrorDescription.ShouldBe("Client authentication failed")
        );
    }

    [Test]
    public async Task GetAsync_AuthenticationFailure_NullBody_ThrowsPingenApiAuthErrorException()
    {
        var identityHandler = new MockHttpMessageHandler(HttpStatusCode.Unauthorized, "null");
        var apiHandler = new MockHttpMessageHandler(HttpStatusCode.OK, "{}");

        var config = CreateConfig();
        var httpClients = CreateHttpClients(identityHandler, apiHandler);
        var handler = new PingenConnectionHandler(config, httpClients);

        var ex = await Should.ThrowAsync<PingenApiAuthErrorException>(async () => await handler.GetAsync("letters", (ApiPagingRequest?)null));
        ex.ShouldSatisfyAllConditions(() => ex.Message.ShouldContain("Failed to obtain access token"), () => ex.AuthError.ShouldBeNull());
    }

    [Test]
    public async Task GetAsync_RateLimitExceeded_ReturnsFailureWithRetryAfter()
    {
        var tokenJson = PingenSerialisationHelper.Serialize(
            new
            {
                access_token = "test-token",
                token_type = "Bearer",
                expires_in = 3600,
            }
        );

        var identityHandler = new MockHttpMessageHandler(HttpStatusCode.OK, tokenJson);
        var apiHandler = new MockHttpMessageHandler(
            (_, _) =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                {
                    Content = new StringContent(
                        "{\"errors\":[{\"code\":\"429\",\"title\":\"Too Many Requests\",\"detail\":\"Rate limit exceeded\",\"source\":{\"pointer\":\"\",\"parameter\":\"\"}}]}"
                    ),
                };
                response.Headers.TryAddWithoutValidation("Retry-After", "30");
                response.Headers.Add("x-ratelimit-limit", "100");
                response.Headers.Add("x-ratelimit-remaining", "0");
                return Task.FromResult(response);
            }
        );

        var config = CreateConfig();
        var httpClients = CreateHttpClients(identityHandler, apiHandler);
        var handler = new PingenConnectionHandler(config, httpClients);

        var result = await handler.GetAsync("letters", (ApiPagingRequest?)null);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.RetryAfter.ShouldBe(30),
            () => result.RateLimitLimit.ShouldBe(100),
            () => result.RateLimitRemaining.ShouldBe(0)
        );
    }

    [Test]
    public async Task GetAsync_UnauthorizedApiResponse_ReturnsFailureResult()
    {
        var tokenJson = PingenSerialisationHelper.Serialize(
            new
            {
                access_token = "test-token",
                token_type = "Bearer",
                expires_in = 3600,
            }
        );

        var identityHandler = new MockHttpMessageHandler(HttpStatusCode.OK, tokenJson);
        var apiHandler = new MockHttpMessageHandler(
            (_, _) =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent(
                        "{\"errors\":[{\"code\":\"401\",\"title\":\"Unauthorized\",\"detail\":\"Invalid or expired token\",\"source\":{\"pointer\":\"\",\"parameter\":\"\"}}]}"
                    ),
                };
                return Task.FromResult(response);
            }
        );

        var config = CreateConfig();
        var httpClients = CreateHttpClients(identityHandler, apiHandler);
        var handler = new PingenConnectionHandler(config, httpClients);

        var result = await handler.GetAsync("letters", (ApiPagingRequest?)null);

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeFalse(), () => result.ApiError.ShouldNotBeNull());
    }

    [Test]
    public async Task GetAsync_ExpiredToken_ReAuthenticates()
    {
        var authCallCount = 0;
        var identityHandler = new MockHttpMessageHandler(
            (_, _) =>
            {
                Interlocked.Increment(ref authCallCount);
                var tokenJson = PingenSerialisationHelper.Serialize(
                    new
                    {
                        access_token = $"token-{authCallCount}",
                        token_type = "Bearer",
                        expires_in = authCallCount == 1 ? 61 : 3600,
                    }
                );
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(tokenJson) });
            }
        );

        string? lastCapturedToken = null;
        var apiHandler = new MockHttpMessageHandler(
            (request, _) =>
            {
                lastCapturedToken = request.Headers.Authorization?.Parameter;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":[]}") });
            }
        );

        var config = CreateConfig();
        var httpClients = CreateHttpClients(identityHandler, apiHandler);
        var handler = new PingenConnectionHandler(config, httpClients);

        // First request triggers initial authentication
        await handler.GetAsync("letters", (ApiPagingRequest?)null);
        authCallCount.ShouldBe(1);

        // Wait for token to expire (expires_in=61, minus 1-minute buffer = ~1 second valid)
        await Task.Delay(2000);

        // Second request should trigger re-authentication
        await handler.GetAsync("letters", (ApiPagingRequest?)null);
        authCallCount.ShouldBe(2);
        lastCapturedToken.ShouldBe("token-2");
    }

    [Test]
    public async Task GetAsync_TokenWithLifetimeShorterThanBuffer_ThrowsInvalidOperationException()
    {
        var tokenJson = PingenSerialisationHelper.Serialize(
            new
            {
                access_token = "short-lived-token",
                token_type = "Bearer",
                expires_in = 30,
            }
        );

        var identityHandler = new MockHttpMessageHandler(HttpStatusCode.OK, tokenJson);
        var apiHandler = new MockHttpMessageHandler(HttpStatusCode.OK, "{\"data\":[]}");

        var config = CreateConfig();
        var httpClients = CreateHttpClients(identityHandler, apiHandler);
        var handler = new PingenConnectionHandler(config, httpClients);

        var ex = await Should.ThrowAsync<InvalidOperationException>(async () => await handler.GetAsync("letters", (ApiPagingRequest?)null));
        ex.Message.ShouldContain("Invalid access token object received");
    }

    [Test]
    public async Task GetAsync_AfterReAuthentication_NewTokenIsReusedForSubsequentRequests()
    {
        var authCallCount = 0;
        var identityHandler = new MockHttpMessageHandler(
            (_, _) =>
            {
                Interlocked.Increment(ref authCallCount);
                var tokenJson = PingenSerialisationHelper.Serialize(
                    new
                    {
                        access_token = $"token-{authCallCount}",
                        token_type = "Bearer",
                        expires_in = authCallCount == 1 ? 61 : 3600,
                    }
                );
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(tokenJson) });
            }
        );

        var capturedTokens = new List<string?>();
        var apiHandler = new MockHttpMessageHandler(
            (request, _) =>
            {
                capturedTokens.Add(request.Headers.Authorization?.Parameter);
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":[]}") });
            }
        );

        var config = CreateConfig();
        var httpClients = CreateHttpClients(identityHandler, apiHandler);
        var handler = new PingenConnectionHandler(config, httpClients);

        // First request authenticates with token-1 (expires_in=61, just > 60s buffer)
        await handler.GetAsync("letters", (ApiPagingRequest?)null);

        // Wait for token-1 to fall inside the 1-minute buffer, forcing re-authentication
        await Task.Delay(2000);

        // Second request triggers re-authentication and obtains token-2 (expires_in=3600)
        await handler.GetAsync("letters", (ApiPagingRequest?)null);

        // Third request must reuse the already-valid token-2 - no new authentication
        await handler.GetAsync("letters", (ApiPagingRequest?)null);

        authCallCount.ShouldBe(2);
        capturedTokens.Count.ShouldBe(3);
        capturedTokens[0].ShouldBe("token-1");
        capturedTokens[1].ShouldBe("token-2");
        capturedTokens[2].ShouldBe("token-2");
    }

    [Test]
    public async Task GetAsync_CancelledCancellationToken_ThrowsOperationCanceledException()
    {
        var tokenJson = PingenSerialisationHelper.Serialize(
            new
            {
                access_token = "test-token",
                token_type = "Bearer",
                expires_in = 3600,
            }
        );

        var identityHandler = new MockHttpMessageHandler(HttpStatusCode.OK, tokenJson);
        var apiHandler = new MockHttpMessageHandler(
            (_, ct) =>
            {
                ct.ThrowIfCancellationRequested();
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":[]}") });
            }
        );

        var config = CreateConfig();
        var httpClients = CreateHttpClients(identityHandler, apiHandler);
        var handler = new PingenConnectionHandler(config, httpClients);

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Should.ThrowAsync<OperationCanceledException>(async () => await handler.GetAsync("letters", (ApiPagingRequest?)null, cts.Token));
    }

    [Test]
    public async Task GetAsync_CancellationDuringInFlightRequest_ThrowsOperationCanceledException()
    {
        var tokenJson = PingenSerialisationHelper.Serialize(
            new
            {
                access_token = "test-token",
                token_type = "Bearer",
                expires_in = 3600,
            }
        );

        using var cts = new CancellationTokenSource();
        var apiCallStarted = new ManualResetEventSlim(false);

        var identityHandler = new MockHttpMessageHandler(HttpStatusCode.OK, tokenJson);
        var apiHandler = new MockHttpMessageHandler(
            async (_, ct) =>
            {
                apiCallStarted.Set();
                await Task.Delay(Timeout.Infinite, ct);
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":[]}") };
            }
        );

        var config = CreateConfig();
        var httpClients = CreateHttpClients(identityHandler, apiHandler);
        var handler = new PingenConnectionHandler(config, httpClients);

        var requestTask = handler.GetAsync("letters", (ApiPagingRequest?)null, cts.Token);

        apiCallStarted.Wait(TimeSpan.FromSeconds(5)).ShouldBeTrue();
        await cts.CancelAsync();

        await Should.ThrowAsync<OperationCanceledException>(async () => await requestTask);
    }

    [Test]
    public async Task SendExternalRequestAsync_CancelledCancellationToken_ThrowsOperationCanceledException()
    {
        var externalHandler = new MockHttpMessageHandler(
            (_, ct) =>
            {
                ct.ThrowIfCancellationRequested();
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("ok") });
            }
        );

        var config = CreateConfig();
        var httpClients = CreateHttpClients(externalHandler: externalHandler);
        var handler = new PingenConnectionHandler(config, httpClients);

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, "https://external.example.com/file.pdf");

        await Should.ThrowAsync<OperationCanceledException>(async () => await handler.SendExternalRequestAsync(request, cts.Token));
    }

    [Test]
    public async Task GetAsync_RateLimitExceeded_WithoutRetryAfter_HasNullRetryAfter()
    {
        var tokenJson = PingenSerialisationHelper.Serialize(
            new
            {
                access_token = "test-token",
                token_type = "Bearer",
                expires_in = 3600,
            }
        );

        var identityHandler = new MockHttpMessageHandler(HttpStatusCode.OK, tokenJson);
        var apiHandler = new MockHttpMessageHandler(
            (_, _) =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                {
                    Content = new StringContent(
                        "{\"errors\":[{\"code\":\"429\",\"title\":\"Too Many Requests\",\"detail\":\"Rate limit exceeded\",\"source\":{\"pointer\":\"\",\"parameter\":\"\"}}]}"
                    ),
                };
                response.Headers.Add("x-ratelimit-limit", "100");
                response.Headers.Add("x-ratelimit-remaining", "0");
                return Task.FromResult(response);
            }
        );

        var config = CreateConfig();
        var httpClients = CreateHttpClients(identityHandler, apiHandler);
        var handler = new PingenConnectionHandler(config, httpClients);

        var result = await handler.GetAsync("letters", (ApiPagingRequest?)null);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeFalse(),
            () => result.RetryAfter.ShouldBeNull(),
            () => result.RateLimitLimit.ShouldBe(100),
            () => result.RateLimitRemaining.ShouldBe(0)
        );
    }

    [Test]
    public async Task GetAsync_MalformedRateLimitHeaders_DoesNotThrowAndUsesDefaults()
    {
        var tokenJson = PingenSerialisationHelper.Serialize(
            new
            {
                access_token = "test-token",
                token_type = "Bearer",
                expires_in = 3600,
            }
        );

        var identityHandler = new MockHttpMessageHandler(HttpStatusCode.OK, tokenJson);
        var apiHandler = new MockHttpMessageHandler(
            (_, _) =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":[]}") };
                response.Headers.Add("x-ratelimit-limit", "not-a-number");
                response.Headers.Add("x-ratelimit-remaining", "garbage");
                response.Headers.Add("x-ratelimit-reset", "abc");
                response.Headers.TryAddWithoutValidation("Retry-After", "soon");
                return Task.FromResult(response);
            }
        );

        var config = CreateConfig();
        var httpClients = CreateHttpClients(identityHandler, apiHandler);
        var handler = new PingenConnectionHandler(config, httpClients);

        var result = await handler.GetAsync("letters", (ApiPagingRequest?)null);

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.RateLimitLimit.ShouldBe(0),
            () => result.RateLimitRemaining.ShouldBe(0),
            () => result.RateLimitReset.ShouldBeNull(),
            () => result.RetryAfter.ShouldBeNull()
        );
    }

    [Test]
    public async Task ConcurrentRequests_DifferentOrganisationIds_UseDifferentUrlPaths()
    {
        var tokenJson = PingenSerialisationHelper.Serialize(
            new
            {
                access_token = "shared-token",
                token_type = "Bearer",
                expires_in = 3600,
            }
        );

        Uri? capturedUriA = null;
        Uri? capturedUriB = null;

        var identityHandlerA = new MockHttpMessageHandler(HttpStatusCode.OK, tokenJson);
        var apiHandlerA = new MockHttpMessageHandler(
            (request, _) =>
            {
                capturedUriA = request.RequestUri;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":[]}") });
            }
        );

        var identityHandlerB = new MockHttpMessageHandler(HttpStatusCode.OK, tokenJson);
        var apiHandlerB = new MockHttpMessageHandler(
            (request, _) =>
            {
                capturedUriB = request.RequestUri;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":[]}") });
            }
        );

        var configA = CreateConfig(defaultOrganisationId: "org-A");
        var configB = CreateConfig(defaultOrganisationId: "org-B");

        var handlerA = new PingenConnectionHandler(configA, CreateHttpClients(identityHandlerA, apiHandlerA));
        var handlerB = new PingenConnectionHandler(configB, CreateHttpClients(identityHandlerB, apiHandlerB));

        await Task.WhenAll(handlerA.GetAsync("letters", (ApiPagingRequest?)null), handlerB.GetAsync("letters", (ApiPagingRequest?)null));

        capturedUriA.ShouldNotBeNull();
        capturedUriB.ShouldNotBeNull();
        capturedUriA!.AbsolutePath.ShouldBe("/organisations/org-A/letters");
        capturedUriB!.AbsolutePath.ShouldBe("/organisations/org-B/letters");
    }

    [TestCase("deliveries/letters", "/organisations/test-org/deliveries/letters")]
    [TestCase("deliveries/letters/events/issues", "/organisations/test-org/deliveries/letters/events/issues")]
    [TestCase("file-upload", "/file-upload")]
    [TestCase("user", "/user")]
    [TestCase("organisations", "/organisations")]
    public async Task GetAsync_NestedOrganisationScopedPath_IsPrefixedCorrectly(string requestPath, string expectedAbsolutePath)
    {
        var tokenJson = PingenSerialisationHelper.Serialize(
            new
            {
                access_token = "test-token",
                token_type = "Bearer",
                expires_in = 3600,
            }
        );

        Uri? capturedUri = null;
        var identityHandler = new MockHttpMessageHandler(HttpStatusCode.OK, tokenJson);
        var apiHandler = new MockHttpMessageHandler(
            (request, _) =>
            {
                capturedUri = request.RequestUri;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":[]}") });
            }
        );

        var handler = new PingenConnectionHandler(CreateConfig(defaultOrganisationId: "test-org"), CreateHttpClients(identityHandler, apiHandler));

        await handler.GetAsync(requestPath, (ApiPagingRequest?)null);

        capturedUri.ShouldNotBeNull();
        capturedUri!.AbsolutePath.ShouldBe(expectedAbsolutePath);
    }

    [Test]
    public async Task SetOrganisationId_BetweenRequests_UsesNewOrganisationIdInUrlPath()
    {
        var tokenJson = PingenSerialisationHelper.Serialize(
            new
            {
                access_token = "test-token",
                token_type = "Bearer",
                expires_in = 3600,
            }
        );

        var capturedPaths = new List<string>();
        var identityHandler = new MockHttpMessageHandler(HttpStatusCode.OK, tokenJson);
        var apiHandler = new MockHttpMessageHandler(
            (request, _) =>
            {
                capturedPaths.Add(request.RequestUri!.AbsolutePath);
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":[]}") });
            }
        );

        var config = CreateConfig(defaultOrganisationId: "initial-org");
        var handler = new PingenConnectionHandler(config, CreateHttpClients(identityHandler, apiHandler));

        await handler.GetAsync("letters", (ApiPagingRequest?)null);
        handler.SetOrganisationId("new-org");
        await handler.GetAsync("letters", (ApiPagingRequest?)null);

        capturedPaths.Count.ShouldBe(2);
        capturedPaths[0].ShouldBe("/organisations/initial-org/letters");
        capturedPaths[1].ShouldBe("/organisations/new-org/letters");
    }

    [Test]
    public async Task GetAsync_NonOrganisationEndpoint_DoesNotInjectOrganisationIdInPath()
    {
        var tokenJson = PingenSerialisationHelper.Serialize(
            new
            {
                access_token = "test-token",
                token_type = "Bearer",
                expires_in = 3600,
            }
        );

        Uri? capturedUri = null;
        var identityHandler = new MockHttpMessageHandler(HttpStatusCode.OK, tokenJson);
        var apiHandler = new MockHttpMessageHandler(
            (request, _) =>
            {
                capturedUri = request.RequestUri;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":{}}") });
            }
        );

        var config = CreateConfig(defaultOrganisationId: "some-org");
        var handler = new PingenConnectionHandler(config, CreateHttpClients(identityHandler, apiHandler));

        await handler.GetAsync("user", (ApiRequest?)null);

        capturedUri.ShouldNotBeNull();
        capturedUri!.AbsolutePath.ShouldBe("/user");
    }

    [Test]
    public async Task GetAsync_FileUploadEndpoint_DoesNotInjectOrganisationIdInPath()
    {
        Uri? capturedUri = null;
        var handler = CreateAuthenticatedHandler(
            (request, _) =>
            {
                capturedUri = request.RequestUri;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":{}}") });
            },
            defaultOrganisationId: "some-org"
        );

        await handler.GetAsync("file-upload", (ApiPagingRequest?)null);

        capturedUri.ShouldNotBeNull();
        capturedUri!.AbsolutePath.ShouldBe("/file-upload");
    }

    [Test]
    public async Task GetAsync_OrganisationsEndpoint_DoesNotInjectOrganisationIdInPath()
    {
        Uri? capturedUri = null;
        var handler = CreateAuthenticatedHandler(
            (request, _) =>
            {
                capturedUri = request.RequestUri;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":{}}") });
            },
            defaultOrganisationId: "some-org"
        );

        await handler.GetAsync("organisations", (ApiPagingRequest?)null);

        capturedUri.ShouldNotBeNull();
        capturedUri!.AbsolutePath.ShouldBe("/organisations");
    }

    [TestCase("letters", "/organisations/some-org/letters")]
    [TestCase("batches", "/organisations/some-org/batches")]
    [TestCase("webhooks", "/organisations/some-org/webhooks")]
    public async Task GetAsync_OrganisationScopedEndpoint_InjectsOrganisationIdInPath(string requestPath, string expectedAbsolutePath)
    {
        Uri? capturedUri = null;
        var handler = CreateAuthenticatedHandler(
            (request, _) =>
            {
                capturedUri = request.RequestUri;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":[]}") });
            },
            defaultOrganisationId: "some-org"
        );

        await handler.GetAsync(requestPath, (ApiPagingRequest?)null);

        capturedUri.ShouldNotBeNull();
        capturedUri!.AbsolutePath.ShouldBe(expectedAbsolutePath);
    }

    [Test]
    public async Task GetAsync_PathStartingWithNonOrgEndpointPrefix_DoesNotInjectOrganisationIdInPath()
    {
        Uri? capturedUri = null;
        var handler = CreateAuthenticatedHandler(
            (request, _) =>
            {
                capturedUri = request.RequestUri;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":{}}") });
            },
            defaultOrganisationId: "some-org"
        );

        await handler.GetAsync("userful-fake-endpoint", (ApiPagingRequest?)null);

        capturedUri.ShouldNotBeNull();
        capturedUri!.AbsolutePath.ShouldBe("/userful-fake-endpoint");
    }

    [Test]
    public async Task PostAsync_WithIdempotencyKey_SetsIdempotencyKeyHeader()
    {
        const string idempotencyKey = "post-key-abc-123";
        string? capturedIdempotencyKey = null;
        var handler = CreateAuthenticatedHandler(
            (request, _) =>
            {
                capturedIdempotencyKey = request.Headers.TryGetValues(ApiHeaderNames.IdempotencyKey, out var values) ? values.First() : null;
                return Task.FromResult(
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{\"data\":{\"id\":\"wh-1\",\"type\":\"webhooks\",\"attributes\":{}}}"),
                    }
                );
            }
        );

        await handler.PostAsync<SingleResult<Data<Webhook>>, DataPost<WebhookCreate>>("webhooks", CreateWebhookDataPost(), idempotencyKey);

        capturedIdempotencyKey.ShouldBe(idempotencyKey);
    }

    [Test]
    public async Task PostAsync_WithoutIdempotencyKey_OmitsIdempotencyKeyHeader()
    {
        var hasIdempotencyKeyHeader = true;
        var handler = CreateAuthenticatedHandler(
            (request, _) =>
            {
                hasIdempotencyKeyHeader = request.Headers.Contains(ApiHeaderNames.IdempotencyKey);
                return Task.FromResult(
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{\"data\":{\"id\":\"wh-1\",\"type\":\"webhooks\",\"attributes\":{}}}"),
                    }
                );
            }
        );

        await handler.PostAsync<SingleResult<Data<Webhook>>, DataPost<WebhookCreate>>("webhooks", CreateWebhookDataPost());

        hasIdempotencyKeyHeader.ShouldBeFalse();
    }

    [Test]
    public async Task PatchAsync_WithBody_WithIdempotencyKey_SetsIdempotencyKeyHeader()
    {
        const string idempotencyKey = "patch-with-body-key-456";
        string? capturedIdempotencyKey = null;
        var handler = CreateAuthenticatedHandler(
            (request, _) =>
            {
                capturedIdempotencyKey = request.Headers.TryGetValues(ApiHeaderNames.IdempotencyKey, out var values) ? values.First() : null;
                return Task.FromResult(
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{\"data\":{\"id\":\"wh-1\",\"type\":\"webhooks\",\"attributes\":{}}}"),
                    }
                );
            }
        );

        await handler.PatchAsync<SingleResult<Data<Webhook>>, DataPatch<WebhookCreate>>("webhooks/wh-1", CreateWebhookDataPatch("wh-1"), idempotencyKey);

        capturedIdempotencyKey.ShouldBe(idempotencyKey);
    }

    [Test]
    public async Task PatchAsync_WithBody_WithoutIdempotencyKey_OmitsIdempotencyKeyHeader()
    {
        var hasIdempotencyKeyHeader = true;
        var handler = CreateAuthenticatedHandler(
            (request, _) =>
            {
                hasIdempotencyKeyHeader = request.Headers.Contains(ApiHeaderNames.IdempotencyKey);
                return Task.FromResult(
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{\"data\":{\"id\":\"wh-1\",\"type\":\"webhooks\",\"attributes\":{}}}"),
                    }
                );
            }
        );

        await handler.PatchAsync<SingleResult<Data<Webhook>>, DataPatch<WebhookCreate>>("webhooks/wh-1", CreateWebhookDataPatch("wh-1"));

        hasIdempotencyKeyHeader.ShouldBeFalse();
    }

    [Test]
    public async Task PatchAsync_NoBody_WithIdempotencyKey_SetsIdempotencyKeyHeader()
    {
        const string idempotencyKey = "patch-no-body-key-789";
        string? capturedIdempotencyKey = null;
        var handler = CreateAuthenticatedHandler(
            (request, _) =>
            {
                capturedIdempotencyKey = request.Headers.TryGetValues(ApiHeaderNames.IdempotencyKey, out var values) ? values.First() : null;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") });
            }
        );

        await handler.PatchAsync("letters/letter-1/cancel", idempotencyKey);

        capturedIdempotencyKey.ShouldBe(idempotencyKey);
    }

    [Test]
    public async Task PatchAsync_NoBody_WithoutIdempotencyKey_OmitsIdempotencyKeyHeader()
    {
        var hasIdempotencyKeyHeader = true;
        var handler = CreateAuthenticatedHandler(
            (request, _) =>
            {
                hasIdempotencyKeyHeader = request.Headers.Contains(ApiHeaderNames.IdempotencyKey);
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") });
            }
        );

        await handler.PatchAsync("letters/letter-1/cancel");

        hasIdempotencyKeyHeader.ShouldBeFalse();
    }

    [Test]
    public async Task PostAsync_IdempotentReplayedTrueHeader_SetsIdempotentReplayedFlag()
    {
        var handler = CreateAuthenticatedHandler(
            (_, _) =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"data\":{\"id\":\"wh-1\",\"type\":\"webhooks\",\"attributes\":{}}}"),
                };
                response.Headers.Add(ApiHeaderNames.IdempotentReplayed, "true");
                return Task.FromResult(response);
            }
        );

        var result = await handler.PostAsync<SingleResult<Data<Webhook>>, DataPost<WebhookCreate>>("webhooks", CreateWebhookDataPost(), "replay-key");

        result.IdempotentReplayed.ShouldBeTrue();
    }

    private static DataPost<WebhookCreate> CreateWebhookDataPost() =>
        new()
        {
            Type = "webhooks",
            Attributes = new WebhookCreate
            {
                EventCategory = WebhookEventCategory.Issues,
                Url = new Uri("https://example.com/webhook"),
                SigningKey = "signing-key",
            },
        };

    private static DataPatch<WebhookCreate> CreateWebhookDataPatch(string id) =>
        new()
        {
            Id = id,
            Type = "webhooks",
            Attributes = new WebhookCreate
            {
                EventCategory = WebhookEventCategory.Issues,
                Url = new Uri("https://example.com/webhook"),
                SigningKey = "signing-key",
            },
        };

    private static PingenConnectionHandler CreateAuthenticatedHandler(
        Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> apiHandlerFunc,
        string defaultOrganisationId = "test-org-id"
    )
    {
        var tokenJson = PingenSerialisationHelper.Serialize(
            new
            {
                access_token = "test-token",
                token_type = "Bearer",
                expires_in = 3600,
            }
        );

        var identityHandler = new MockHttpMessageHandler(HttpStatusCode.OK, tokenJson);
        var apiHandler = new MockHttpMessageHandler(apiHandlerFunc);

        var config = CreateConfig(defaultOrganisationId: defaultOrganisationId);
        return new PingenConnectionHandler(config, CreateHttpClients(identityHandler, apiHandler));
    }

    private static IPingenConfiguration CreateConfig(
        string baseUri = "https://api.example.com/",
        string identityUri = "https://identity.example.com/",
        string defaultOrganisationId = "test-org-id"
    )
    {
        return new PingenConfiguration
        {
            BaseUri = baseUri,
            IdentityUri = identityUri,
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            DefaultOrganisationId = defaultOrganisationId,
        };
    }

    private static PingenHttpClients CreateHttpClients(
        MockHttpMessageHandler? identityHandler = null,
        MockHttpMessageHandler? apiHandler = null,
        MockHttpMessageHandler? externalHandler = null
    )
    {
        var identityClient = new HttpClient(identityHandler ?? new MockHttpMessageHandler(HttpStatusCode.OK, "{}"))
        {
            BaseAddress = new Uri("https://identity.example.com/"),
        };

        var apiClient = new HttpClient(apiHandler ?? new MockHttpMessageHandler(HttpStatusCode.OK, "{}")) { BaseAddress = new Uri("https://api.example.com/") };

        var externalClient = new HttpClient(externalHandler ?? new MockHttpMessageHandler(HttpStatusCode.OK, "{}"));

        return new PingenHttpClients(identityClient, apiClient, externalClient);
    }
}

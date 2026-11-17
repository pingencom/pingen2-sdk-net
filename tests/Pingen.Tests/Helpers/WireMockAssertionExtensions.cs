using WireMock.Server;

namespace Pingen.Tests.Helpers;

internal static class WireMockAssertionExtensions
{
    internal static void VerifyCalled(this WireMockServer server, string path, string method = "GET", int times = 1)
    {
        int count = server.LogEntries.Count(e =>
            e.RequestMessage?.Path == path && string.Equals(e.RequestMessage?.Method, method, StringComparison.OrdinalIgnoreCase)
        );

        count.ShouldBe(times, $"Expected '{method} {path}' to be called {times} time(s) but was called {count} time(s).");
    }

    internal static void VerifyCalledAtLeastOnce(this WireMockServer server, string path, string method = "GET")
    {
        int count = server.LogEntries.Count(e =>
            e.RequestMessage?.Path == path && string.Equals(e.RequestMessage?.Method, method, StringComparison.OrdinalIgnoreCase)
        );

        count.ShouldBeGreaterThanOrEqualTo(1, $"Expected '{method} {path}' to be called at least once but was never called.");
    }

    internal static void VerifyNotCalled(this WireMockServer server, string path, string method = "GET")
    {
        int count = server.LogEntries.Count(e =>
            e.RequestMessage?.Path == path && string.Equals(e.RequestMessage?.Method, method, StringComparison.OrdinalIgnoreCase)
        );

        count.ShouldBe(0, $"Expected '{method} {path}' to never be called but was called {count} time(s).");
    }
}

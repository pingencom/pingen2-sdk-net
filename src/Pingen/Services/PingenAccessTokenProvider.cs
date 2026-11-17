using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Pingen.Models;

namespace Pingen.Services;

/// <summary>
/// Process-wide cache of OAuth access tokens, keyed so each set of credentials (identity endpoint + client id)
/// gets its own token. Registered as a singleton in DI so the <c>client_credentials</c> token is fetched once
/// and shared across request scopes, instead of the scoped <see cref="PingenConnectionHandler"/> (scoped
/// because it also holds the per-request organisation id) re-authenticating on the first call of every scope.
/// Keying by client id keeps different tenants' tokens isolated: a caller only ever receives the token minted
/// for its own credentials.
/// </summary>
internal sealed class PingenAccessTokenProvider
{
    private sealed class Slot
    {
        public readonly SemaphoreSlim Gate = new(1, 1);

        public AccessToken? Token;
    }

    private readonly ConcurrentDictionary<string, Slot> _slots = new();

    /// <summary>
    /// Return a valid bearer token for the given credentials, authenticating via <paramref name="login"/> only
    /// when there is no cached token or the cached one is within a minute of expiry.
    /// </summary>
    public async Task<string> GetValidTokenAsync(string cacheKey, Func<Task<AccessToken?>> login)
    {
        var slot = _slots.GetOrAdd(cacheKey, static _ => new Slot());

        if (IsValid(slot.Token))
            return slot.Token!.Token;

        await EnterGate(slot);
        try
        {
            // Double-check now that we hold the gate: another caller may have authenticated meanwhile.
            if (IsValid(slot.Token))
                return slot.Token!.Token;

            slot.Token = await Authenticate(login);
            return slot.Token.Token;
        }
        finally
        {
            slot.Gate.Release();
        }
    }

    /// <summary>
    /// Force a one-time re-authentication after a <c>401</c>. De-duplicated against concurrent callers: if
    /// another caller already replaced the stale token with a fresh valid one, that is returned instead of
    /// logging in again.
    /// </summary>
    // Excluded from coverage: the concurrent de-duplication branch needs two callers racing on the same stale
    // token, which is not deterministically reproducible in a unit test.
    [ExcludeFromCodeCoverage]
    public async Task<string> ForceRefreshAsync(string cacheKey, string? staleToken, Func<Task<AccessToken?>> login)
    {
        var slot = _slots.GetOrAdd(cacheKey, static _ => new Slot());

        await EnterGate(slot);
        try
        {
            if (slot.Token is not null && slot.Token.Token != staleToken && IsValid(slot.Token))
                return slot.Token.Token;

            slot.Token = await Authenticate(login);
            return slot.Token.Token;
        }
        finally
        {
            slot.Gate.Release();
        }
    }

    private static async Task<AccessToken> Authenticate(Func<Task<AccessToken?>> login)
    {
        var token = await login();
        if (!IsValid(token))
            throw new InvalidOperationException("Invalid access token object received");

        return token!;
    }

    private static bool IsValid(AccessToken? token) => token is not null && token.ExpiresAt.AddMinutes(-1) > DateTime.UtcNow;

    /// <summary>
    /// Acquire a slot's gate, throwing if it cannot be entered within a bounded time (a guard against a stuck
    /// semaphore, not reachable under normal operation).
    /// </summary>
    [ExcludeFromCodeCoverage]
    private static async Task EnterGate(Slot slot)
    {
        if (!await slot.Gate.WaitAsync(TimeSpan.FromSeconds(10)))
            throw new InvalidOperationException("Authentication semaphore entrance timeout");
    }
}

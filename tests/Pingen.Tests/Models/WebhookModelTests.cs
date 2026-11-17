using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Models.Webhooks;
using Pingen.Abstractions.Models.Webhooks.Views;

namespace Pingen.Tests.Models;

[TestFixture]
public sealed class WebhookModelTests
{
    [Test]
    public void Webhook_ToString_RedactsSigningKey()
    {
        var webhook = new Webhook(WebhookEventCategory.Issues, new Uri("https://example.com/hook"), "hmac-signing-secret");

        var text = webhook.ToString();

        text.ShouldSatisfyAllConditions(
            () => text.ShouldNotContain("hmac-signing-secret"),
            () => text.ShouldContain("SigningKey = ***"),
            () => text.ShouldContain("https://example.com/hook")
        );
    }

    [Test]
    public void WebhookCreate_ToString_RedactsSigningKey()
    {
        var create = new WebhookCreate
        {
            EventCategory = WebhookEventCategory.Issues,
            Url = new Uri("https://example.com/hook"),
            SigningKey = "hmac-signing-secret",
        };

        var text = create.ToString();

        text.ShouldSatisfyAllConditions(() => text.ShouldNotContain("hmac-signing-secret"), () => text.ShouldContain("SigningKey = ***"));
    }
}

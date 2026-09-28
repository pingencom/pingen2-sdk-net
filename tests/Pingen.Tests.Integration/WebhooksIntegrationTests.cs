using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Models.Api.Embedded;
using Pingen.Abstractions.Models.Webhooks.Views;

namespace Pingen.Tests.Integration;

[TestFixture]
[Category("Integration")]
public sealed class WebhooksIntegrationTests : IntegrationSupport
{
    private const string Url = "https://httpbin.org/post";

    private string _webhookId = null!;

    [Test]
    [Order(1)]
    public async Task ListWebhooks()
    {
        var collection = await Client.Webhooks.GetPage();
        AssertSuccess(collection);
    }

    [Test]
    [Order(2)]
    public async Task CreateWebhook()
    {
        var created = await Client.Webhooks.Create(
            new DataPost<WebhookCreate>
            {
                Attributes = new WebhookCreate
                {
                    EventCategory = WebhookEventCategory.Issues,
                    Url = new Uri(Url),
                    SigningKey = "integration-test-signing-key-32c",
                },
            }
        );
        AssertSuccess(created);
        created.Data!.Data.Id.ShouldNotBeNullOrEmpty();
        created.Data.Data.Attributes.Url.ShouldBe(new Uri(Url));
        _webhookId = created.Data.Data.Id;
    }

    [Test]
    [Order(3)]
    public async Task GetWebhookById()
    {
        var details = await Client.Webhooks.Get(_webhookId);
        AssertSuccess(details);
        details.Data!.Data.Id.ShouldBe(_webhookId);
        details.Data.Data.Attributes.Url.ShouldBe(new Uri(Url));
    }

    [Test]
    [Order(4)]
    public async Task DeleteWebhook()
    {
        var deleted = await Client.Webhooks.Delete(_webhookId);
        AssertSuccess(deleted);

        var afterDelete = await Client.Webhooks.Get(_webhookId);
        afterDelete.IsSuccess.ShouldBeFalse();
    }
}

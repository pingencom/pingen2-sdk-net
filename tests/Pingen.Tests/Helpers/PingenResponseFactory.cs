using System.Text.Json;
using Bogus;

namespace Pingen.Tests.Helpers;

internal static class PingenResponseFactory
{
    private const string DefaultOrganisationId = "test-org-id-001";
    private static readonly Faker _faker = new();

    // ── Letters ─────────────────────────────────────────────────────────────

    internal static string SingleLetter(string? id = null, string? organisationId = null)
    {
        id ??= Guid.NewGuid().ToString();
        organisationId ??= DefaultOrganisationId;

        return JsonApiStubHelper.SingleResponse(
            id,
            "letters",
            LetterAttributes(),
            LetterRelationships(organisationId),
            JsonApiStubHelper.MetaWithAbilities(LetterAbilitiesObject())
        );
    }

    internal static string LetterCollection(int count = 3, string? organisationId = null, int currentPage = 1, int lastPage = 1)
    {
        organisationId ??= DefaultOrganisationId;

        IEnumerable<(string, object, object?, object?)> items = Enumerable
            .Range(0, count)
            .Select(_ => (Guid.NewGuid().ToString(), LetterAttributes(), (object?)LetterRelationships(organisationId), (object?)null));

        return JsonApiStubHelper.CollectionResponse(items, "letters", currentPage, lastPage, total: count);
    }

    internal static string LetterEventCollection(int count = 3, string? letterId = null, int currentPage = 1, int lastPage = 1)
    {
        string parentLetterId = letterId ?? Guid.NewGuid().ToString();

        IEnumerable<(string, object, object?, object?)> items = Enumerable
            .Range(0, count)
            .Select(_ =>
                (
                    Guid.NewGuid().ToString(),
                    LetterEventAttributes(),
                    (object?)new { letter = JsonApiStubHelper.RelatedSingle(parentLetterId, "letters") },
                    (object?)null
                )
            );

        return JsonApiStubHelper.CollectionResponse(items, "letters_events", currentPage, lastPage, total: count);
    }

    internal static string SingleLetterPriceCalculator(string? id = null, decimal price = 1.50m)
    {
        id ??= Guid.NewGuid().ToString();

        return JsonApiStubHelper.SingleResponse(id, "letter_price_calculator", new { currency = "CHF", price });
    }

    // ── Batches ──────────────────────────────────────────────────────────────

    internal static string SingleBatch(string? id = null, string? organisationId = null)
    {
        id ??= Guid.NewGuid().ToString();
        organisationId ??= DefaultOrganisationId;

        return JsonApiStubHelper.SingleResponse(
            id,
            "batches",
            BatchAttributes(),
            new { organisation = JsonApiStubHelper.RelatedSingle(organisationId, "organisations"), events = JsonApiStubHelper.RelatedMany() },
            JsonApiStubHelper.MetaWithAbilities(
                new
                {
                    cancel = "ok",
                    delete = "ok",
                    submit = "ok",
                    edit = "ok",
                }
            )
        );
    }

    internal static string BatchCollection(int count = 3, string? organisationId = null, int currentPage = 1, int lastPage = 1)
    {
        organisationId ??= DefaultOrganisationId;

        IEnumerable<(string, object, object?, object?)> items = Enumerable
            .Range(0, count)
            .Select(_ =>
                (
                    Guid.NewGuid().ToString(),
                    BatchAttributes(),
                    (object?)new { organisation = JsonApiStubHelper.RelatedSingle(organisationId, "organisations"), events = JsonApiStubHelper.RelatedMany() },
                    (object?)null
                )
            );

        return JsonApiStubHelper.CollectionResponse(items, "batches", currentPage, lastPage, total: count);
    }

    // ── Files ────────────────────────────────────────────────────────────────

    internal static string FileUploadPath(string? id = null)
    {
        id ??= Guid.NewGuid().ToString();
        string url = $"https://s3.example.com/upload/{id}";

        return JsonApiStubHelper.SingleResponse(
            id,
            "file_uploads",
            new
            {
                url,
                url_signature = _faker.Random.AlphaNumeric(64),
                expires_at = DateTimeOffset.UtcNow.AddMinutes(30).ToString("o"),
            }
        );
    }

    // ── Webhooks ─────────────────────────────────────────────────────────────

    internal static string SingleWebhook(string? id = null, string? organisationId = null)
    {
        id ??= Guid.NewGuid().ToString();
        organisationId ??= DefaultOrganisationId;

        return JsonApiStubHelper.SingleResponse(
            id,
            "webhooks",
            WebhookAttributes(),
            new { organisation = JsonApiStubHelper.RelatedSingle(organisationId, "organisations") }
        );
    }

    internal static string WebhookCollection(int count = 3, string? organisationId = null, int currentPage = 1, int lastPage = 1)
    {
        organisationId ??= DefaultOrganisationId;

        IEnumerable<(string, object, object?, object?)> items = Enumerable
            .Range(0, count)
            .Select(_ =>
                (
                    Guid.NewGuid().ToString(),
                    WebhookAttributes(),
                    (object?)new { organisation = JsonApiStubHelper.RelatedSingle(organisationId, "organisations") },
                    (object?)null
                )
            );

        return JsonApiStubHelper.CollectionResponse(items, "webhooks", currentPage, lastPage, total: count);
    }

    // ── Organisations ────────────────────────────────────────────────────────

    internal static string SingleOrganisation(string? id = null)
    {
        id ??= Guid.NewGuid().ToString();

        return JsonApiStubHelper.SingleResponse(
            id,
            "organisations",
            OrganisationAttributes(),
            new { associations = JsonApiStubHelper.RelatedMany() },
            JsonApiStubHelper.MetaWithAbilities(new { manage = "ok" })
        );
    }

    internal static string OrganisationCollection(int count = 3, int currentPage = 1, int lastPage = 1)
    {
        IEnumerable<(string, object, object?, object?)> items = Enumerable
            .Range(0, count)
            .Select(_ => (Guid.NewGuid().ToString(), OrganisationAttributes(), (object?)new { associations = JsonApiStubHelper.RelatedMany() }, (object?)null));

        return JsonApiStubHelper.CollectionResponse(items, "organisations", currentPage, lastPage, total: count);
    }

    // ── Users ────────────────────────────────────────────────────────────────

    internal static string SingleUser(string? id = null)
    {
        id ??= Guid.NewGuid().ToString();

        return JsonApiStubHelper.SingleResponse(
            id,
            "users",
            UserAttributes(),
            new { associations = JsonApiStubHelper.RelatedMany(), notifications = JsonApiStubHelper.RelatedMany() },
            JsonApiStubHelper.MetaWithAbilities(new { reach = "ok", act = "ok" })
        );
    }

    internal static string UserAssociationCollection(int count = 3, int currentPage = 1, int lastPage = 1)
    {
        IEnumerable<(string, object, object?, object?)> items = Enumerable
            .Range(0, count)
            .Select(i =>
                (
                    Guid.NewGuid().ToString(),
                    UserAssociationAttributes(),
                    (object?)new { organisation = JsonApiStubHelper.RelatedSingle($"org-for-assoc-{i}", "organisations") },
                    (object?)JsonApiStubHelper.MetaWithOrganisationAbilities(new { reach = "ok", act = "ok" }, new { manage = "ok" })
                )
            );

        return JsonApiStubHelper.CollectionResponse(items, "associations", currentPage, lastPage, total: count);
    }

    internal static string SingleUserAssociation(string? id = null)
    {
        id ??= Guid.NewGuid().ToString();

        return JsonApiStubHelper.SingleResponse(
            id,
            "associations",
            UserAssociationAttributes(),
            new { organisation = JsonApiStubHelper.RelatedSingle("org-for-assoc", "organisations") },
            JsonApiStubHelper.MetaWithOrganisationAbilities(new { reach = "ok", act = "ok" }, new { manage = "ok" })
        );
    }

    // ── Error Responses ──────────────────────────────────────────────────────

    internal static string ErrorResponse(string? title = null, string? detail = null, string status = "422")
    {
        return JsonSerializer.Serialize(
            new
            {
                errors = new[]
                {
                    new
                    {
                        status,
                        title = title ?? "Unprocessable Entity",
                        detail = detail ?? "The request could not be processed.",
                    },
                },
            }
        );
    }

    // ── Private Attribute Builders ───────────────────────────────────────────

    private static object LetterAttributes() =>
        new
        {
            status = _faker.PickRandom("valid", "invalid", "pending"),
            file_original_name = _faker.System.FileName("pdf"),
            file_pages = _faker.Random.Int(1, 10),
            address = _faker.Address.StreetAddress(),
            address_position = _faker.PickRandom("left", "right"),
            country = _faker.PickRandom("CH", "DE", "AT"),
            delivery_product = _faker.PickRandom("cheap", "priority"),
            print_mode = _faker.PickRandom("simplex", "duplex"),
            print_spectrum = _faker.PickRandom("grayscale", "color"),
            price_currency = "CHF",
            price_value = Math.Round(_faker.Random.Double(0.5, 5.0), 2),
            created_at = _faker.Date.Past().ToString("o"),
            updated_at = _faker.Date.Recent(30).ToString("o"),
        };

    private static object LetterEventAttributes() =>
        new
        {
            code = _faker.PickRandom("submitted", "delivered", "issue_detected", "issue_resolved", "sent"),
            name = _faker.Lorem.Sentence(3),
            producer = _faker.PickRandom("PostAG", "DPAG", "AustrianPost"),
            location = _faker.Address.City(),
            has_image = _faker.Random.Bool(),
            data = Array.Empty<string>(),
            emitted_at = _faker.Date.Recent(7).ToString("o"),
            created_at = _faker.Date.Recent(7).ToString("o"),
            updated_at = _faker.Date.Recent(7).ToString("o"),
        };

    private static object BatchAttributes() =>
        new
        {
            name = _faker.Commerce.ProductName(),
            icon = _faker.PickRandom("flat", "priority"),
            status = _faker.PickRandom("valid", "invalid", "pending"),
            file_original_name = _faker.System.FileName("pdf"),
            letter_count = _faker.Random.Int(1, 100),
            address_position = _faker.PickRandom("left", "right"),
            print_mode = _faker.PickRandom("simplex", "duplex"),
            print_spectrum = _faker.PickRandom("grayscale", "color"),
            price_currency = "CHF",
            price_value = Math.Round(_faker.Random.Double(1.0, 50.0), 2),
            created_at = _faker.Date.Past().ToString("o"),
            updated_at = _faker.Date.Recent(30).ToString("o"),
        };

    private static object WebhookAttributes() =>
        new
        {
            event_category = _faker.PickRandom("issues", "undeliverable", "sent"),
            url = _faker.Internet.UrlWithPath(),
            signing_key = _faker.Random.AlphaNumeric(32),
        };

    private static object OrganisationAttributes() =>
        new
        {
            name = _faker.Company.CompanyName(),
            status = _faker.PickRandom("active", "inactive"),
            plan = _faker.PickRandom("free", "professional", "enterprise"),
            billing_mode = _faker.PickRandom("prepaid", "postpaid"),
            billing_currency = "CHF",
            billing_balance = Math.Round(_faker.Random.Double(0, 1000), 2),
            default_country = _faker.PickRandom("CH", "DE", "AT"),
            default_address_position = _faker.PickRandom("left", "right"),
            data_retention_addresses = _faker.Random.Int(30, 365),
            data_retention_pdf = _faker.Random.Int(30, 365),
            color = _faker.Internet.Color(),
            created_at = _faker.Date.Past().ToString("o"),
            updated_at = _faker.Date.Recent(30).ToString("o"),
        };

    private static object UserAttributes() =>
        new
        {
            email = _faker.Internet.Email(),
            first_name = _faker.Name.FirstName(),
            last_name = _faker.Name.LastName(),
            status = _faker.PickRandom("active", "pending", "blocked"),
            language = _faker.PickRandom("en", "de", "fr", "it"),
            created_at = _faker.Date.Past().ToString("o"),
            updated_at = _faker.Date.Recent(30).ToString("o"),
        };

    private static object UserAssociationAttributes() =>
        new
        {
            role = _faker.PickRandom("owner", "manager"),
            status = _faker.PickRandom("active", "pending", "blocked"),
            created_at = _faker.Date.Past().ToString("o"),
            updated_at = _faker.Date.Recent(30).ToString("o"),
        };

    private static object LetterRelationships(string organisationId) =>
        new
        {
            organisation = JsonApiStubHelper.RelatedSingle(organisationId, "organisations"),
            events = JsonApiStubHelper.RelatedMany(),
            batch = JsonApiStubHelper.RelatedSingle(Guid.NewGuid().ToString(), "batches"),
        };

    private static object LetterAbilitiesObject() =>
        new
        {
            cancel = "ok",
            delete = "ok",
            submit = "ok",
            send_simplex = "ok",
            edit = "ok",
            get_pdf_raw = "ok",
            get_pdf_validation = "ok",
            change_paper_type = "ok",
            change_window_position = "ok",
            create_coverpage = "ok",
            fix_overwrite_restricted_areas = "ok",
            fix_coverpage = "ok",
            fix_regular_paper = "ok",
        };

    // ── Emails ───────────────────────────────────────────────────────────────

    internal static string SingleEmail(string? id = null, string? organisationId = null)
    {
        id ??= Guid.NewGuid().ToString();
        organisationId ??= DefaultOrganisationId;

        return JsonApiStubHelper.SingleResponse(
            id,
            "emails",
            EmailAttributes(),
            DeliverableRelationships(organisationId),
            JsonApiStubHelper.MetaWithAbilities(new { cancel = "ok", delete = "ok" })
        );
    }

    internal static string EmailCollection(int count = 3, string? organisationId = null, int currentPage = 1, int lastPage = 1)
    {
        organisationId ??= DefaultOrganisationId;

        IEnumerable<(string, object, object?, object?)> items = Enumerable
            .Range(0, count)
            .Select(_ => (Guid.NewGuid().ToString(), EmailAttributes(), (object?)DeliverableRelationships(organisationId), (object?)null));

        return JsonApiStubHelper.CollectionResponse(items, "emails", currentPage, lastPage, total: count);
    }

    internal static string EmailEventCollection(int count = 3, string? emailId = null, int currentPage = 1, int lastPage = 1)
    {
        string parentId = emailId ?? Guid.NewGuid().ToString();

        IEnumerable<(string, object, object?, object?)> items = Enumerable
            .Range(0, count)
            .Select(_ =>
                (
                    Guid.NewGuid().ToString(),
                    LetterEventAttributes(),
                    (object?)new { email = JsonApiStubHelper.RelatedSingle(parentId, "emails") },
                    (object?)null
                )
            );

        return JsonApiStubHelper.CollectionResponse(items, "deliverables_events", currentPage, lastPage, total: count);
    }

    // ── Ebills ───────────────────────────────────────────────────────────────

    internal static string SingleEbill(string? id = null, string? organisationId = null)
    {
        id ??= Guid.NewGuid().ToString();
        organisationId ??= DefaultOrganisationId;

        return JsonApiStubHelper.SingleResponse(
            id,
            "ebills",
            EbillAttributes(),
            DeliverableRelationships(organisationId),
            JsonApiStubHelper.MetaWithAbilities(
                new
                {
                    cancel = "ok",
                    delete = "ok",
                    submit = "ok",
                }
            )
        );
    }

    internal static string EbillCollection(int count = 3, string? organisationId = null, int currentPage = 1, int lastPage = 1)
    {
        organisationId ??= DefaultOrganisationId;

        IEnumerable<(string, object, object?, object?)> items = Enumerable
            .Range(0, count)
            .Select(_ => (Guid.NewGuid().ToString(), EbillAttributes(), (object?)DeliverableRelationships(organisationId), (object?)null));

        return JsonApiStubHelper.CollectionResponse(items, "ebills", currentPage, lastPage, total: count);
    }

    internal static string EbillEventCollection(int count = 3, string? ebillId = null, int currentPage = 1, int lastPage = 1)
    {
        string parentId = ebillId ?? Guid.NewGuid().ToString();

        IEnumerable<(string, object, object?, object?)> items = Enumerable
            .Range(0, count)
            .Select(_ =>
                (
                    Guid.NewGuid().ToString(),
                    LetterEventAttributes(),
                    (object?)new { ebill = JsonApiStubHelper.RelatedSingle(parentId, "ebills") },
                    (object?)null
                )
            );

        return JsonApiStubHelper.CollectionResponse(items, "deliverables_events", currentPage, lastPage, total: count);
    }

    // ── Batch events & statistics ────────────────────────────────────────────

    internal static string BatchEventCollection(int count = 3, string? batchId = null, int currentPage = 1, int lastPage = 1)
    {
        string parentId = batchId ?? Guid.NewGuid().ToString();

        IEnumerable<(string, object, object?, object?)> items = Enumerable
            .Range(0, count)
            .Select(_ =>
                (
                    Guid.NewGuid().ToString(),
                    BatchEventAttributes(),
                    (object?)new { batch = JsonApiStubHelper.RelatedSingle(parentId, "batches") },
                    (object?)null
                )
            );

        return JsonApiStubHelper.CollectionResponse(items, "batches_events", currentPage, lastPage, total: count);
    }

    internal static string SingleBatchStatistics(string? id = null)
    {
        id ??= Guid.NewGuid().ToString();

        return JsonApiStubHelper.SingleResponse(id, "batch_details_statistics", BatchStatisticsAttributes());
    }

    private static object EmailAttributes() =>
        new
        {
            status = _faker.PickRandom("sent", "validating", "cancelled"),
            file_original_name = _faker.System.FileName("pdf"),
            file_pages = _faker.Random.Int(1, 10),
            recipient_identifier = _faker.Internet.Email(),
            price_currency = "CHF",
            price_value = Math.Round(_faker.Random.Double(0.0, 2.0), 2),
            source = _faker.PickRandom("app", "api", "batch"),
            submitted_at = _faker.Date.Recent(7).ToString("o"),
            created_at = _faker.Date.Past().ToString("o"),
            updated_at = _faker.Date.Recent(30).ToString("o"),
        };

    private static object EbillAttributes() =>
        new
        {
            status = _faker.PickRandom("sent", "validating", "cancelled"),
            file_original_name = _faker.System.FileName("pdf"),
            file_pages = _faker.Random.Int(1, 10),
            recipient_identifier = _faker.Random.AlphaNumeric(10),
            recipient_address = _faker.Address.FullAddress(),
            invoice_number = _faker.Random.Replace("INV-#####"),
            invoice_date = _faker.Date.Recent(30).ToString("yyyy-MM-dd"),
            invoice_due_date = _faker.Date.Soon(30).ToString("yyyy-MM-dd"),
            invoice_value = Math.Round(_faker.Random.Double(10, 1000), 2),
            invoice_currency = "CHF",
            invoice_iban = _faker.Finance.Iban(),
            invoice_address = _faker.Address.FullAddress(),
            invoice_reference = _faker.Random.AlphaNumeric(12),
            price_currency = "CHF",
            price_value = Math.Round(_faker.Random.Double(0.0, 2.0), 2),
            source = _faker.PickRandom("app", "api", "batch"),
            submitted_at = _faker.Date.Recent(7).ToString("o"),
            created_at = _faker.Date.Past().ToString("o"),
            updated_at = _faker.Date.Recent(30).ToString("o"),
        };

    private static object BatchEventAttributes() =>
        new
        {
            code = _faker.PickRandom("submitted", "processing", "sent"),
            name = _faker.Lorem.Sentence(3),
            producer = _faker.PickRandom("PostAG", "DPAG"),
            location = _faker.Address.City(),
            data = Array.Empty<string>(),
            emitted_at = _faker.Date.Recent(7).ToString("o"),
            created_at = _faker.Date.Recent(7).ToString("o"),
            updated_at = _faker.Date.Recent(7).ToString("o"),
        };

    private static object BatchStatisticsAttributes() =>
        new
        {
            letter_validating = _faker.Random.Int(0, 5),
            letter_groups = new[] { new { name = _faker.Commerce.Department(), count = _faker.Random.Int(1, 50) } },
            letter_countries = new[] { new { country = _faker.PickRandom("CH", "DE", "AT"), count = _faker.Random.Int(1, 50) } },
            letter_regions = new[] { new { country = _faker.PickRandom("ZH", "BE", "VD"), count = _faker.Random.Int(1, 50) } },
        };

    private static object DeliverableRelationships(string organisationId) =>
        new
        {
            organisation = JsonApiStubHelper.RelatedSingle(organisationId, "organisations"),
            events = JsonApiStubHelper.RelatedMany(),
            batch = JsonApiStubHelper.RelatedSingle(Guid.NewGuid().ToString(), "batches"),
        };
}

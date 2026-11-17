using Microsoft.Extensions.DependencyInjection;
using Pingen.Interfaces;
using Pingen.Interfaces.Connectors;
using Pingen.Services;
using Pingen.Services.Connectors;

namespace Pingen.AspNetCore;

/// <summary>
/// Pingen service collection extension for dependency injection
/// </summary>
public static class PingenServiceCollection
{
    /// <summary>
    /// Adds the configuration, handler and services to the service collection, selecting the production or
    /// staging environment URLs automatically via the <paramref name="useStaging"/> switch.
    /// </summary>
    /// <param name="clientId">OAuth client id.</param>
    /// <param name="clientSecret">OAuth client secret.</param>
    /// <param name="defaultOrganisationId">Default organisation id.</param>
    /// <param name="useStaging">When <c>true</c>, target the staging environment; otherwise production (default).</param>
    public static IServiceCollection AddPingenServices(
        this IServiceCollection services,
        string clientId,
        string clientSecret,
        string defaultOrganisationId,
        bool useStaging = false
    ) => services.AddPingenServices(PingenConfiguration.Create(clientId, clientSecret, defaultOrganisationId, useStaging));

    /// <summary>
    /// Adds the configuration, handler and services to the service collection.
    /// </summary>
    public static IServiceCollection AddPingenServices(
        this IServiceCollection services,
        string baseUri,
        string identityUri,
        string clientId,
        string clientSecret,
        string defaultOrganisationId
    )
    {
        return services.AddPingenServices(
            new PingenConfiguration
            {
                BaseUri = baseUri,
                IdentityUri = identityUri,
                ClientId = clientId,
                ClientSecret = clientSecret,
                DefaultOrganisationId = defaultOrganisationId,
            }
        );
    }

    /// <summary>
    /// Adds the configuration, handler and services to the service collection.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public static IServiceCollection AddPingenServices(this IServiceCollection services, IPingenConfiguration pingenConfiguration)
    {
        services.AddSingleton(pingenConfiguration);
        services.AddHttpClient(
            PingenHttpClients.Names.Identity,
            identityClient =>
            {
                identityClient.BaseAddress = new(pingenConfiguration.IdentityUri);
                identityClient.DefaultRequestHeaders.Accept.Clear();
                identityClient.DefaultRequestHeaders.Accept.Add(new("application/x-www-form-urlencoded"));
                identityClient.DefaultRequestHeaders.UserAgent.ParseAdd(PingenHttpClients.UserAgent);
            }
        );
        services
            .AddHttpClient(
                PingenHttpClients.Names.Api,
                apiClient =>
                {
                    apiClient.BaseAddress = new(pingenConfiguration.BaseUri);
                    apiClient.DefaultRequestHeaders.UserAgent.ParseAdd(PingenHttpClients.UserAgent);
                }
            )
            .ConfigurePrimaryHttpMessageHandler(p => new HttpClientHandler { AllowAutoRedirect = false });
        services.AddHttpClient(PingenHttpClients.Names.Files);
        services.AddScoped<PingenHttpClients>(sp => new(sp.GetRequiredService<IHttpClientFactory>()));
        // Singleton token cache shared across scopes; the handler stays scoped for its per-request organisation id.
        services.AddSingleton<PingenAccessTokenProvider>();
        services.AddScoped<IPingenConnectionHandler>(sp => new PingenConnectionHandler(
            sp.GetRequiredService<IPingenConfiguration>(),
            sp.GetRequiredService<PingenHttpClients>(),
            sp.GetRequiredService<PingenAccessTokenProvider>()
        ));
        services.AddScoped<ILetterService, LetterService>();
        services.AddScoped<IBatchService, BatchService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEbillService, EbillService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IOrganisationService, OrganisationService>();
        services.AddScoped<IWebhookService, WebhookService>();
        services.AddScoped<IFilesService, FilesService>();
        services.AddScoped<ILetterEventService, LetterEventService>();
        services.AddScoped<IBatchEventService, BatchEventService>();
        services.AddScoped<IEmailEventService, EmailEventService>();
        services.AddScoped<IEbillEventService, EbillEventService>();
        services.AddScoped<IPingenApiClient, PingenApiClient>();

        return services;
    }
}

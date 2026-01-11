using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Stripe;
using StripeApiWrapper.Services;

namespace StripeApiWrapper.Configuration;

/// <summary>
/// Extension methods for registering Stripe API services with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Stripe API wrapper services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure Stripe options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddStripeServices(
        this IServiceCollection services,
        Action<StripeOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.Configure(configureOptions);

        return services.AddStripeServicesCore();
    }

    /// <summary>
    /// Adds Stripe API wrapper services to the service collection using configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration section containing Stripe options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddStripeServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.Configure<StripeOptions>(configuration.GetSection(StripeOptions.SectionName));

        return services.AddStripeServicesCore();
    }

    private static IServiceCollection AddStripeServicesCore(this IServiceCollection services)
    {
        // Register the Stripe client configuration
        services.AddSingleton<IStripeClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<StripeOptions>>().Value;
            options.Validate();

            return new StripeClient(
                apiKey: options.SecretKey,
                clientId: null,
                httpClient: null,
                apiBase: null,
                filesBase: null,
                connectBase: null,
                meterEventsBase: null);
        });

        // Register service implementations
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<ICustomerService, Services.CustomerService>();
        // TODO: Register additional services in future iterations
        // services.AddScoped<ISubscriptionService, SubscriptionService>();
        // services.AddScoped<IInvoiceService, InvoiceService>();

        return services;
    }
}

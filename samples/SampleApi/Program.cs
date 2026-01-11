using StripeApiWrapper.Configuration;
using StripeApiWrapper.Webhooks;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Stripe services
builder.Services.AddStripeServices(builder.Configuration);
builder.Services.AddStripeWebhooks();

var app = builder.Build();

// Map endpoints
app.MapControllers();

app.MapGet("/", () => "StripeApiWrapper Sample API");

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    version = StripeApiWrapper.StripeApiWrapperMarker.Version
}));

// Map Stripe webhook endpoint
app.MapStripeWebhook("/webhooks/stripe");

app.Run();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// TODO: Add StripeApiWrapper services once configuration layer is implemented
// builder.Services.AddStripeServices(options => { ... });

var app = builder.Build();

app.MapGet("/", () => "StripeApiWrapper Sample API");

app.MapGet("/health", () => Results.Ok(new { status = "healthy", version = StripeApiWrapper.StripeApiWrapperMarker.Version }));

app.Run();

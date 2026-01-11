# StripeApiWrapper

A reusable .NET library that wraps Stripe API functionality, providing a clean interface for payments, customers, subscriptions, invoices, and webhook handling.

## Features

- **Payment Processing** - Create, confirm, capture, and refund payments
- **Customer Management** - Create, update, delete customers and manage payment methods
- **Subscription Management** - Full subscription lifecycle support
- **Invoice Management** - Generate, finalize, pay, and void invoices
- **Webhook Processing** - Signature verification and event routing middleware

## Installation

```bash
dotnet add package LukeWeaver.StripeApiWrapper
```

## Quick Start

### 1. Configure Services

```csharp
using StripeApiWrapper.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStripeServices(options =>
{
    options.SecretKey = builder.Configuration["Stripe:SecretKey"];
    options.WebhookSecret = builder.Configuration["Stripe:WebhookSecret"];
});

var app = builder.Build();

app.UseStripeWebhooks("/webhooks/stripe");

app.Run();
```

### 2. Use Services

```csharp
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment(PaymentRequest request)
    {
        var result = await _paymentService.CreatePaymentAsync(request);
        return Ok(result);
    }
}
```

## Configuration

Add to your `appsettings.json`:

```json
{
  "Stripe": {
    "SecretKey": "sk_test_...",
    "WebhookSecret": "whsec_...",
    "PublishableKey": "pk_test_..."
  }
}
```

## Requirements

- .NET 10.0+ or .NET Standard 2.0+
- Stripe.net 50.x

## License

MIT License

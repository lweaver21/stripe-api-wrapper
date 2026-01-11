# W.Core.Stripe

[![CI Build](https://github.com/lweaver21/W.Core.Stripe/actions/workflows/ci.yml/badge.svg)](https://github.com/lweaver21/W.Core.Stripe/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/W.Core.Stripe.svg)](https://www.nuget.org/packages/W.Core.Stripe)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A reusable .NET library that wraps Stripe API functionality, providing a clean interface for payments, customers, subscriptions, invoices, and webhook handling.

> **Note:** This package was generated with the assistance of AI (Claude by Anthropic). The code has been tested and reviewed, but please report any issues you encounter.

## Features

- **Payment Processing** - Create, confirm, capture, refund, and cancel payments with 3D Secure support
- **Customer Management** - Full CRUD operations for customers with payment method management
- **Subscription Management** - Complete subscription lifecycle including trials, updates, and cancellations
- **Invoice Management** - Generate, finalize, pay, void, and send invoices
- **Webhook Processing** - Signature verification, event routing, and custom handler support
- **Dependency Injection** - First-class support for Microsoft.Extensions.DependencyInjection
- **Multi-targeting** - Supports .NET 10.0 and .NET Standard 2.0

## Installation

```bash
dotnet add package W.Core.Stripe
```

Or via Package Manager:

```powershell
Install-Package W.Core.Stripe
```

## Quick Start

### 1. Configure Services

```csharp
using StripeApiWrapper.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Option 1: Configure with action
builder.Services.AddStripeServices(options =>
{
    options.SecretKey = builder.Configuration["Stripe:SecretKey"];
    options.WebhookSecret = builder.Configuration["Stripe:WebhookSecret"];
});

// Option 2: Configure from IConfiguration
builder.Services.AddStripeServices(builder.Configuration);

var app = builder.Build();

// Map webhook endpoint (requires .NET 10.0)
app.MapStripeWebhook("/webhooks/stripe");

app.Run();
```

### 2. Add Configuration

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

> **Security Note:** Never commit API keys to source control. Use environment variables or a secrets manager in production.

### 3. Use Services

```csharp
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment(CreatePaymentDto dto)
    {
        var request = new PaymentRequest
        {
            Amount = dto.Amount,
            Currency = "usd",
            CustomerId = dto.CustomerId,
            PaymentMethodId = dto.PaymentMethodId
        };

        var result = await _paymentService.CreatePaymentAsync(request);
        return Ok(result);
    }
}
```

## Detailed Usage

### Payment Service

```csharp
// Inject IPaymentService
private readonly IPaymentService _paymentService;

// Create a payment
var result = await _paymentService.CreatePaymentAsync(new PaymentRequest
{
    Amount = 2000, // $20.00 in cents
    Currency = "usd",
    CustomerId = "cus_xxx",
    PaymentMethodId = "pm_xxx",
    Description = "Order #1234",
    CaptureImmediately = true,
    IdempotencyKey = Guid.NewGuid().ToString()
});

// Confirm a payment (for manual confirmation flow)
var confirmed = await _paymentService.ConfirmPaymentAsync(
    paymentIntentId: "pi_xxx",
    paymentMethodId: "pm_xxx"
);

// Capture an authorized payment
var captured = await _paymentService.CapturePaymentAsync(
    paymentIntentId: "pi_xxx",
    amountToCapture: 1500 // Partial capture
);

// Refund a payment
var refundId = await _paymentService.RefundPaymentAsync(
    paymentIntentId: "pi_xxx",
    amount: 500, // Partial refund
    reason: "requested_by_customer"
);

// Cancel a payment
var cancelled = await _paymentService.CancelPaymentAsync(
    paymentIntentId: "pi_xxx",
    cancellationReason: "abandoned"
);

// Get payment details
var payment = await _paymentService.GetPaymentAsync("pi_xxx");
```

### Customer Service

```csharp
// Inject ICustomerService
private readonly ICustomerService _customerService;

// Create a customer
var customer = await _customerService.CreateCustomerAsync(new CustomerDto
{
    Email = "customer@example.com",
    Name = "John Doe",
    Phone = "+1234567890",
    Address = new AddressDto
    {
        Line1 = "123 Main St",
        City = "San Francisco",
        State = "CA",
        PostalCode = "94102",
        Country = "US"
    }
});

// Update a customer
var updated = await _customerService.UpdateCustomerAsync(
    customerId: "cus_xxx",
    new CustomerDto { Name = "Jane Doe" }
);

// Get a customer
var existing = await _customerService.GetCustomerAsync("cus_xxx");

// Delete a customer
var deleted = await _customerService.DeleteCustomerAsync("cus_xxx");

// Attach a payment method
var paymentMethodId = await _customerService.AttachPaymentMethodAsync(
    customerId: "cus_xxx",
    paymentMethodId: "pm_xxx",
    setAsDefault: true
);

// List payment methods
var methods = await _customerService.ListPaymentMethodsAsync(
    customerId: "cus_xxx",
    type: "card"
);
```

### Subscription Service

```csharp
// Inject ISubscriptionService
private readonly ISubscriptionService _subscriptionService;

// Create a subscription
var subscription = await _subscriptionService.CreateSubscriptionAsync(
    new CreateSubscriptionRequest
    {
        CustomerId = "cus_xxx",
        PriceId = "price_xxx",
        Quantity = 1,
        TrialDays = 14,
        PaymentMethodId = "pm_xxx"
    }
);

// Update a subscription
var updated = await _subscriptionService.UpdateSubscriptionAsync(
    subscriptionId: "sub_xxx",
    priceId: "price_new",
    quantity: 2
);

// Cancel at period end
var cancelled = await _subscriptionService.CancelSubscriptionAsync(
    subscriptionId: "sub_xxx",
    cancelAtPeriodEnd: true
);

// Cancel immediately
var cancelledNow = await _subscriptionService.CancelSubscriptionAsync(
    subscriptionId: "sub_xxx",
    cancelAtPeriodEnd: false
);

// Resume a cancelled subscription
var resumed = await _subscriptionService.ResumeSubscriptionAsync("sub_xxx");

// List customer subscriptions
var subscriptions = await _subscriptionService.ListSubscriptionsAsync(
    customerId: "cus_xxx",
    status: "active"
);
```

### Invoice Service

```csharp
// Inject IInvoiceService
private readonly IInvoiceService _invoiceService;

// Create an invoice
var invoice = await _invoiceService.CreateInvoiceAsync(new CreateInvoiceRequest
{
    CustomerId = "cus_xxx",
    Description = "Professional Services",
    CollectionMethod = "send_invoice",
    DaysUntilDue = 30,
    AutoAdvance = false
});

// Finalize an invoice
var finalized = await _invoiceService.FinalizeInvoiceAsync("inv_xxx");

// Send an invoice
var sent = await _invoiceService.SendInvoiceAsync("inv_xxx");

// Pay an invoice
var paid = await _invoiceService.PayInvoiceAsync(
    invoiceId: "inv_xxx",
    paymentMethodId: "pm_xxx"
);

// Void an invoice
var voided = await _invoiceService.VoidInvoiceAsync("inv_xxx");

// List customer invoices
var invoices = await _invoiceService.ListInvoicesAsync(
    customerId: "cus_xxx",
    status: "open",
    limit: 10
);
```

## Webhook Handling

### Setup Webhook Endpoint (.NET 10.0)

```csharp
var app = builder.Build();

// Map the webhook endpoint
app.MapStripeWebhook("/webhooks/stripe");

app.Run();
```

### Create Custom Handlers

```csharp
using Stripe;
using StripeApiWrapper.Webhooks;

public class PaymentSucceededHandler : IWebhookHandler
{
    public string EventType => "payment_intent.succeeded";

    public async Task HandleAsync(Event stripeEvent, CancellationToken cancellationToken)
    {
        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

        // Handle the successful payment
        Console.WriteLine($"Payment succeeded: {paymentIntent.Id}");

        await Task.CompletedTask;
    }
}

public class SubscriptionUpdatedHandler : IWebhookHandler
{
    public string EventType => "customer.subscription.updated";

    public async Task HandleAsync(Event stripeEvent, CancellationToken cancellationToken)
    {
        var subscription = stripeEvent.Data.Object as Subscription;

        // Handle subscription update
        Console.WriteLine($"Subscription updated: {subscription.Id}");

        await Task.CompletedTask;
    }
}

// Catch-all handler
public class AllEventsHandler : IWebhookHandler
{
    public string EventType => "*"; // Handles all events

    public async Task HandleAsync(Event stripeEvent, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Received event: {stripeEvent.Type}");
        await Task.CompletedTask;
    }
}
```

### Register Handlers

```csharp
builder.Services.AddStripeServices(options => { /* ... */ });
builder.Services.AddStripeWebhooks();
builder.Services.AddWebhookHandler<PaymentSucceededHandler>();
builder.Services.AddWebhookHandler<SubscriptionUpdatedHandler>();
```

## Exception Handling

The library provides custom exceptions for better error handling:

```csharp
using StripeApiWrapper.Exceptions;

try
{
    var result = await _paymentService.CreatePaymentAsync(request);
}
catch (PaymentFailedException ex)
{
    // Card was declined or payment failed
    Console.WriteLine($"Payment failed: {ex.DeclineCode}");
    Console.WriteLine($"User message: {ex.UserFacingMessage}");
}
catch (CustomerNotFoundException ex)
{
    // Customer doesn't exist
    Console.WriteLine($"Customer not found: {ex.CustomerId}");
}
catch (SubscriptionException ex)
{
    // Subscription-related error
    Console.WriteLine($"Subscription error: {ex.Reason}");
}
catch (StripeApiException ex)
{
    // General Stripe API error
    Console.WriteLine($"Stripe error: {ex.ErrorCode} - {ex.Message}");
}
```

## Configuration Options

```csharp
public class StripeOptions
{
    public string SecretKey { get; set; }        // Required: Stripe secret key
    public string WebhookSecret { get; set; }    // Required for webhooks
    public string PublishableKey { get; set; }   // Optional: For client-side
}
```

## Supported Frameworks

| Framework | Webhook Middleware | Core Services |
|-----------|-------------------|---------------|
| .NET 10.0 | ✅ | ✅ |
| .NET Standard 2.0 | ❌ | ✅ |

## Development Workflow

This project uses GitHub Actions for CI/CD:

1. **Pull Requests** - Automatically build and test
2. **Merge to Master** - Build, test, and publish to NuGet

### Branch Protection

The `master` branch requires:
- Pull request with 1 approving review
- Passing CI build
- Up-to-date branch

### Publishing a New Version

1. Update version in `src/StripeApiWrapper/StripeApiWrapper.csproj`
2. Update `CHANGELOG.md`
3. Create a PR and merge to master
4. Package is automatically published to NuGet

### Manual Publishing

```bash
# Linux/macOS
./publish-nuget.sh YOUR_NUGET_API_KEY

# Windows PowerShell
.\publish-nuget.ps1 -ApiKey YOUR_NUGET_API_KEY
```

## Requirements

- .NET 10.0+ or .NET Standard 2.0+
- Stripe.net 50.x

## License

MIT License - See [LICENSE](LICENSE) for details.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## Support

- [GitHub Issues](https://github.com/lweaver21/W.Core.Stripe/issues)
- [NuGet Package](https://www.nuget.org/packages/W.Core.Stripe)

---

*This package was created with AI assistance (Claude by Anthropic) and is maintained by Luke Weaver.*

# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-01-10

### Added

- Initial release of StripeApiWrapper
- **Configuration Layer**
  - `StripeOptions` class for API key configuration
  - `AddStripeServices()` extension methods for DI registration
  - Support for both action-based and `IConfiguration`-based configuration

- **Payment Service**
  - `IPaymentService` interface with full payment lifecycle support
  - Create, confirm, capture, refund, and cancel payment intents
  - Idempotency key support for safe retries
  - 3D Secure authentication flow support

- **Customer Service**
  - `ICustomerService` interface for customer management
  - CRUD operations for customers
  - Payment method attachment and management
  - Address and shipping information support

- **Subscription Service**
  - `ISubscriptionService` interface for subscription lifecycle
  - Create, update, cancel, and resume subscriptions
  - Trial period support
  - Quantity and plan changes

- **Invoice Service**
  - `IInvoiceService` interface for invoice operations
  - Create, finalize, pay, void, and send invoices
  - Invoice line item support

- **Webhook Infrastructure**
  - `IWebhookHandler` interface for custom event handlers
  - `WebhookProcessor` for signature verification and event routing
  - `MapStripeWebhook()` endpoint extension for ASP.NET Core
  - Support for multiple handlers per event type

- **Exception Handling**
  - `StripeApiException` base class with error codes
  - `PaymentFailedException` with decline codes and user-friendly messages
  - `CustomerNotFoundException` for missing customer errors
  - `SubscriptionException` with error reasons

- **Sample API**
  - Complete working example with controllers
  - Demonstrates all service operations
  - Error handling patterns

### Dependencies

- Stripe.net 50.x
- Microsoft.Extensions.DependencyInjection.Abstractions 9.x
- Microsoft.Extensions.Options 9.x
- Microsoft.Extensions.Logging.Abstractions 9.x

### Supported Frameworks

- .NET 10.0 (with ASP.NET Core support)
- .NET Standard 2.0 (without webhook middleware)

using Rotinik.Features.Payments;

namespace Rotinik.Tests.Features.Payments;

public static class PaymentDataBuilder
{
    public static CheckoutRequestDto CreateValidCheckoutDto() => new()
    {
        Amount = 29.90m
    };
}
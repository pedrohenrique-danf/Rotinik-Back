using System.Net.Http.Json;
using Rotinik.Features.Payments;

namespace Rotinik.Tests.Features.Payments;

public class PaymentApiClient
{
    private readonly HttpClient _client;

    public PaymentApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<HttpResponseMessage> CheckoutAsync(CheckoutRequestDto dto) 
        => await _client.PostAsJsonAsync("/api/payments/checkout", dto);

    public async Task<HttpResponseMessage> ApprovePaymentMockAsync(string transactionId) 
        => await _client.PostAsync($"/api/payments/webhook-mock/{transactionId}", null);

    public async Task<HttpResponseMessage> CheckStatusAsync(string transactionId) 
        => await _client.GetAsync($"/api/payments/status/{transactionId}");
}
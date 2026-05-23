using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Rotinik.Tests.Core;
using Rotinik.Tests.Features.Users;
using Xunit;

namespace Rotinik.Tests.Features.Payments;

public class PaymentTests : IntegrationTestBase
{
    private readonly UserApiClient _userApi;
    private readonly PaymentApiClient _paymentApi;

    public PaymentTests(CustomApiFactory factory) : base(factory) 
    { 
        _userApi = new UserApiClient(Client);
        _paymentApi = new PaymentApiClient(Client);
    }

    private async Task SetupUserAsync()
    {
        var newUser = UserDataBuilder.CreateValidRegistrationDto();
        await _userApi.RegisterUserAsync(newUser);

        var token = await _userApi.LoginAndGetTokenAsync(newUser.Email, newUser.Password);
        SetToken(token);
    }

    [Fact]
    public async Task PaymentLifecycle_Checkout_Approve_ActivatesPremium()
    {
        await SetupUserAsync();

        var checkoutDto = PaymentDataBuilder.CreateValidCheckoutDto();
        var checkoutResponse = await _paymentApi.CheckoutAsync(checkoutDto);
        
        Assert.Equal(HttpStatusCode.OK, checkoutResponse.StatusCode);
        
        var checkoutJson = await checkoutResponse.Content.ReadFromJsonAsync<JsonElement>();
        var transactionId = checkoutJson.GetProperty("transactionId").GetString();
        
        Assert.NotNull(transactionId);

        var pendingStatusResponse = await _paymentApi.CheckStatusAsync(transactionId);
        var pendingJson = await pendingStatusResponse.Content.ReadFromJsonAsync<JsonElement>();
        
        Assert.Equal("Pending", pendingJson.GetProperty("status").GetString());

        var webhookResponse = await _paymentApi.ApprovePaymentMockAsync(transactionId);
        Assert.Equal(HttpStatusCode.OK, webhookResponse.StatusCode);

        var paidStatusResponse = await _paymentApi.CheckStatusAsync(transactionId);
        var paidJson = await paidStatusResponse.Content.ReadFromJsonAsync<JsonElement>();
        
        Assert.Equal("Paid", paidJson.GetProperty("status").GetString());

        var vipResponse = await Client.GetAsync("/api/user/conteudo-vip");
        Assert.Equal(HttpStatusCode.OK, vipResponse.StatusCode);
    }

    [Fact]
    public async Task ApprovePaymentMock_NonExistentTransaction_ReturnsNotFound()
    {
        var webhookResponse = await _paymentApi.ApprovePaymentMockAsync("tx_invalid_123");
        
        Assert.Equal(HttpStatusCode.NotFound, webhookResponse.StatusCode);
    }

    [Fact]
    public async Task ApprovePaymentMock_AlreadyPaidTransaction_ReturnsConflict()
    {
        await SetupUserAsync();
        
        var checkoutResponse = await _paymentApi.CheckoutAsync(PaymentDataBuilder.CreateValidCheckoutDto());
        var checkoutJson = await checkoutResponse.Content.ReadFromJsonAsync<JsonElement>();
        var transactionId = checkoutJson.GetProperty("transactionId").GetString()!;

        await _paymentApi.ApprovePaymentMockAsync(transactionId);

        var duplicateWebhookResponse = await _paymentApi.ApprovePaymentMockAsync(transactionId);
        
        Assert.Equal(HttpStatusCode.Conflict, duplicateWebhookResponse.StatusCode);
    }

    [Fact]
    public async Task ProtectedRoutes_WithoutToken_ReturnsUnauthorized()
    {
        ClearToken();

        var checkoutResponse = await _paymentApi.CheckoutAsync(PaymentDataBuilder.CreateValidCheckoutDto());
        var statusResponse = await _paymentApi.CheckStatusAsync("tx_123");

        Assert.Equal(HttpStatusCode.Unauthorized, checkoutResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, statusResponse.StatusCode);
    }
}
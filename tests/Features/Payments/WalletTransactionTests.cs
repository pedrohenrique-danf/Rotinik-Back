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
    
    private string _userEmail = string.Empty;
    private string _userPassword = string.Empty;

    public PaymentTests(CustomApiFactory factory) : base(factory) 
    { 
        _userApi = new UserApiClient(Client);
        _paymentApi = new PaymentApiClient(Client);
    }

    private async Task SetupUserAsync()
    {
        var newUser = UserDataBuilder.CreateValidRegistrationDto();
        _userEmail = newUser.Email;
        _userPassword = newUser.Password;

        await _userApi.RegisterUserAsync(newUser);
        var token = await _userApi.LoginAndGetTokenAsync(_userEmail, _userPassword);
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
        
        var webhookResponse = await _paymentApi.ApprovePaymentMockAsync(transactionId!);
        Assert.Equal(HttpStatusCode.OK, webhookResponse.StatusCode);

        var newToken = await _userApi.LoginAndGetTokenAsync(_userEmail, _userPassword); 
        SetToken(newToken);

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
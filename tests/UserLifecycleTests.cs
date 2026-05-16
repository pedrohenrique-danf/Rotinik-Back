using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Rotinik.Data;
using Rotinik.DTOs.User;
using Xunit;

namespace Rotinik.Tests;

// ===================================================================
// 1. CONFIGURAÇÃO DA FÁBRICA (O Servidor com Testcontainers)
// ===================================================================
public class CustomApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // 1. Define qual imagem do Docker vamos usar (versão alpine é mais leve/rápida)
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:15-alpine")
        .WithDatabase("rotinik_test_db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    // 2. Inicia o contêiner no Docker ANTES dos testes começarem
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    // 3. Destrói o contêiner DEPOIS que os testes terminarem
    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Limpa qualquer configuração de banco anterior (como fizemos antes)
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<System.Data.Common.DbConnection>();

            // Adiciona o Postgres apontando dinamicamente para o Contêiner que acabou de nascer
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });

            // Constrói o banco e as tabelas lá dentro antes de rodar a API
            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated(); 
        });
    }
}

// ===================================================================
// 2. A BATERIA DE TESTES
// ===================================================================
public class UserLifecycleTests : IClassFixture<CustomApiFactory>
{
    private readonly HttpClient _client;

    public UserLifecycleTests(CustomApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UserLifecycle_ShouldRegisterLoginUpdateAndDeleteSuccessfully()
    {
        // ==========================================
        // 1. SIGN UP (CREATE)
        // ==========================================
        var newUser = new UserRegistrationDto
        {
            Name = "Test User",
            BirthDate = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UserName = "tester_lifecycle",
            Email = "tester@email.com",
            Password = "Password123!" // Senha forte válida
        };

        var createResponse = await _client.PostAsJsonAsync("/api/user", newUser);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        // ==========================================
        // 2. SIGN IN (LOGIN)
        // ==========================================
        var loginData = new UserLoginDto
        {
            Email = "tester@email.com",
            Password = "Password123!"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/user/login", loginData);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        // Extrai o token JWT da resposta
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var token = loginResult.GetProperty("token").GetString();
        Assert.False(string.IsNullOrEmpty(token));

        // Coloca o token no "cabeçalho" para as próximas requisições
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // ==========================================
        // 3. GET USER (ME)
        // ==========================================
        var getMeResponse = await _client.GetAsync("/api/user/me");
        Assert.Equal(HttpStatusCode.OK, getMeResponse.StatusCode);

        var userData = await getMeResponse.Content.ReadFromJsonAsync<JsonElement>();
        var userId = userData.GetProperty("id").GetInt32();
        Assert.True(userId > 0);

        // ==========================================
        // 4. UPDATE
        // ==========================================
        var updateData = new UserUpdateDto
        {
            Name = "Updated Name",
            BirthDate = new DateTime(1995, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/user/{userId}", updateData);
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        // ==========================================
        // 5. DELETE
        // ==========================================
        var deleteResponse = await _client.DeleteAsync($"/api/user/{userId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // ==========================================
        // 6. FINAL VERIFICATION (Login should fail)
        // ==========================================
        var finalLoginResponse = await _client.PostAsJsonAsync("/api/user/login", loginData);
        Assert.Equal(HttpStatusCode.Unauthorized, finalLoginResponse.StatusCode);
    }

    [Fact]
    public async Task CreateUser_WithWeakPassword_ShouldReturn400BadRequest()
    {
        // Arrange: Preparando dados inválidos
        var weakUser = new UserRegistrationDto
        {
            Name = "Weak User",
            BirthDate = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UserName = "weak_tester",
            Email = "weak@email.com",
            Password = "weak" // Falha no Regex
        };

        // Act: Fazendo a requisição
        var response = await _client.PostAsJsonAsync("/api/user", weakUser);

        // Assert: Validando que o servidor barrou a entrada
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var errorResult = await response.Content.ReadFromJsonAsync<JsonElement>();
        var errorMessage = errorResult.GetProperty("message").GetString();
        Assert.Contains("Password requirements", errorMessage);
    }
}

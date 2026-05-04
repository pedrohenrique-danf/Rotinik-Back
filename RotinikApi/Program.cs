using Microsoft.EntityFrameworkCore;
using RotinikApi.Data;
using RotinikApi.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddOpenApi();

// 2. ADIÇÃO: Política de CORS (Permite que o Angular acesse o C#)
builder.Services.AddCors(options =>
{
    options.AddPolicy("RotinikAppPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Origem do seu Angular
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Banco de dados
builder.Services.AddDbContext<RotinikContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("RotinikConnection"))
);

// Serviços
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "RotinikApi";
        options.Theme = ScalarTheme.DeepSpace;
        options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();
app.UseCors("RotinikAppPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();
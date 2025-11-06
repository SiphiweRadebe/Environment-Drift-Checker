using MudBlazor.Services;
using Environment_Drift_Checker.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations
builder.AddServiceDefaults();

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure HttpClient with proper BaseAddress
builder.Services.AddHttpClient("ApiService", (sp, client) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    // Get the API service URL from Aspire configuration
    var apiUrl = configuration["services:apiservice:https:0"]
              ?? configuration["services:apiservice:http:0"]
              ?? "https://localhost:7320"; // Fallback for local debugging

    client.BaseAddress = new Uri(apiUrl);
    client.Timeout = TimeSpan.FromSeconds(30);

    Console.WriteLine($"✅ API BaseAddress set to: {apiUrl}");
});

// Register the HttpClient as a scoped service for injection
builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("ApiService");
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapDefaultEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
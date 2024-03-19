using Elsa.Studio.Workflows.Designer.Extensions;
using Elsa.Studio.Core.BlazorServer.Extensions;
using Elsa.Studio.Dashboard.Extensions;
using Elsa.Studio.Extensions;
using Elsa.Studio.Shell.Extensions;
using Elsa.Studio.Workflows.Extensions;
using Blazored.LocalStorage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddOptions();

builder.Services.AddMemoryCache();

builder.Services.AddHttpContextAccessor();

// Local storage
builder.Services.AddBlazoredLocalStorage(c => { c.JsonSerializerOptions.WriteIndented = true; });

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(options =>
{
    // Register the root components.
    options.RootComponents.RegisterCustomElsaStudioElements();
}).AddCircuitOptions(options => { options.DetailedErrors = true; });

// Register shell services and modules.
builder.Services.AddCore();
builder.Services.AddShell(options => builder.Configuration.GetSection("Shell").Bind(options));
builder.Services.AddRemoteBackend(
    elsaClient =>
    {
        elsaClient.ApiKey = builder.Configuration["Backend:ApiKey"];
        elsaClient.BaseAddress = new Uri(builder.Configuration["Backend:Url"]!);
    },
    options => builder.Configuration.GetSection("Backend").Bind(options));

builder.Services.AddDashboardModule();
builder.Services.AddWorkflowsModule();

// Configure SignalR.
builder.Services.AddSignalR(options =>
{
    // Set MaximumReceiveMessageSize to handle large workflows.
    options.MaximumReceiveMessageSize = 5 * 1024 * 1000; // 5MB
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
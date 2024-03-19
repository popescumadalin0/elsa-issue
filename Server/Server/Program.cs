using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

var identitySection = builder.Configuration.GetSection("Identity");
var identityTokenSection = identitySection.GetSection("Tokens");

builder.Services.AddLogging();
builder.Services.AddControllers();

builder.Services.AddElsa(elsa =>
{
    // Configure Management layer to use EF Core.
    elsa.UseWorkflowManagement(management => management.UseEntityFrameworkCore());

    // Configure Runtime layer to use EF Core.
    elsa.UseWorkflowRuntime(runtime => runtime.UseEntityFrameworkCore());

    elsa.UseIdentity(identity =>
    {
        identity.IdentityOptions = options => identitySection.Bind(options);
        identity.TokenOptions = options => identityTokenSection.Bind(options);
        identity.UseConfigurationBasedApplicationProvider(options => identitySection.Bind(options));
        identity.UseConfigurationBasedRoleProvider(options => identitySection.Bind(options));
    });

    elsa.UseDefaultAuthentication();

    elsa.UseWorkflowsApi();

    elsa.UseRealTimeWorkflows();

    elsa.UseCSharp();
    elsa.UseJavaScript();
    elsa.UseLiquid();

    elsa.UseHttp();
    elsa.UseScheduling();

    elsa.AddActivitiesFrom<Program>();
    elsa.AddWorkflowsFrom<Program>();
    elsa.AddSwagger();
});
builder.Services.AddRazorPages();


builder.Services.AddCors(cors => cors
    .AddDefaultPolicy(policy => policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithExposedHeaders("x-elsa-workflow-instance-id")));

var app = builder.Build();

app.UseCors();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseWorkflowsApi();
app.UseWorkflows();
app.UseWorkflowsSignalRHubs();

app.MapControllers();

app.MapRazorPages();

app.UseSwaggerGen();
app.UseSwaggerUI();

app.Run();
return;
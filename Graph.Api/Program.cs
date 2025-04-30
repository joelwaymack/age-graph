using Graph.Api.Config;
using Graph.Api.DataAccess;
using Graph.Api.Services;
using Microsoft.SemanticKernel;
using Npgsql;
using Npgsql.Age.Internal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var postgresConfig = builder.Configuration.GetSection(PostgresConfig.ConfigSectionName).Get<PostgresConfig>();
var dataSourceBuilder = new NpgsqlDataSourceBuilder(postgresConfig.ConnectionString);
dataSourceBuilder.AddTypeInfoResolverFactory(new AgtypeResolverFactory());

builder.Services.AddSingleton(dataSourceBuilder.Build());
builder.Services.AddSingleton(builder.Configuration.GetSection(PostgresConfig.ConfigSectionName).Get<PostgresConfig>());
builder.Services.AddScoped<GraphDatabase>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "All",
        builder =>
        {
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
            builder.AllowAnyOrigin();
        });
});

var kernelBuilder = builder.Services.AddKernel();
var aiConfig = builder.Configuration.GetSection(AiConfig.ConfigSectionName).Get<AiConfig>();
kernelBuilder.AddAzureOpenAIChatCompletion(deploymentName: aiConfig.DeploymentName, apiKey: aiConfig.ApiKey, endpoint: aiConfig.Endpoint);
kernelBuilder.Plugins.Services.AddTransient<GraphDatabase>();
kernelBuilder.Plugins.AddFromType<AssetPlugin>("Assets");

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("All");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Graph API")
    .ExcludeFromDescription();

app.MapControllers();

app.Run();
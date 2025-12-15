using Contracts;
using FileAnalysisService.Infrastructure.DI;
using FileAnalysisService.Infrastructure.Persistence;
using FileAnalysisService.UseCases;
using FileAnalysisService.UseCases.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAnalyzeWorkUseCase, AnalyzeWorkUseCase>();
builder.Services.AddScoped<IGetWorkReportsUseCase, GetWorkReportsUseCase>();

var dataDir = Environment.GetEnvironmentVariable("ANALYSIS_DATA_DIR") ?? "/data";
builder.Services.AddInfrastructure(dataDir);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.MapPost("/submissions", async (AnalyzeWorkRequest request, IAnalyzeWorkUseCase useCase, CancellationToken ct) =>
{
    var resp = await useCase.ExecuteAsync(request, ct);
    return Results.Ok(resp);
});

app.MapGet("/works/{workId}/reports", async (string workId, IGetWorkReportsUseCase useCase, CancellationToken ct) =>
{
    var resp = await useCase.ExecuteAsync(workId, ct);
    return Results.Ok(resp);
});

app.Run();
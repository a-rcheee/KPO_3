using ApiGateway.Infrastructure.DI;
using ApiGateway.UseCases;
using ApiGateway.UseCases.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ISubmitWorkUseCase, SubmitWorkUseCase>();
builder.Services.AddScoped<IProcessPendingUseCase, ProcessPendingUseCase>();

var dataDir = Environment.GetEnvironmentVariable("GATEWAY_DATA_DIR") ?? "/data";
var storageUrl = builder.Configuration["Storage:BaseUrl"] ?? "http://file-storage:8080";
var analysisUrl = builder.Configuration["Analysis:BaseUrl"] ?? "http://file-analysis:8080";

builder.Services.AddInfrastructure(dataDir, storageUrl, analysisUrl);

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApiGateway.Infrastructure.GatewayDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/api/works/{workId}/submissions",
        async (string workId, string studentName, IFormFile file, ISubmitWorkUseCase useCase, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(studentName))
            {
                return Results.BadRequest(new { error = "studentName is required" });
            }

            if (file.Length == 0)
            {
                return Results.BadRequest(new { error = "file is required" });
            }

            await using var stream = file.OpenReadStream();
            var result = await useCase.ExecuteAsync(workId, studentName, stream, file.FileName, ct);

            return result.Status == "Analyzed"
                ? Results.Ok(result)
                : Results.Accepted(value: result);
        })
    .Accepts<IFormFile>("multipart/form-data")
    .DisableAntiforgery();


app.MapGet("/api/works/{workId}/reports", async (string workId, IConfiguration cfg, CancellationToken ct) =>
{
    var analysisBase = cfg["Analysis:BaseUrl"] ?? "http://file-analysis:8080";
    using var http = new HttpClient();
    http.BaseAddress = new Uri(analysisBase);

    var resp = await http.GetAsync($"/works/{workId}/reports", ct);
    if (!resp.IsSuccessStatusCode)
    {
        return Results.Problem("Analysis error", statusCode: 502);
    }

    var json = await resp.Content.ReadAsStringAsync(ct);
    return Results.Content(json, "application/json");
});

app.MapPost("/api/pending/process", async (IProcessPendingUseCase useCase, CancellationToken ct) =>
{
    var processed = await useCase.ExecuteAsync(ct);
    return Results.Ok(new { processed });
});

app.Run();
using FileStorageService.Infrastructure;
using FileStorageService.UseCases;
using FileStorageService.UseCases.Abstractions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IStoreFileUseCase, StoreFileUseCase>();
builder.Services.AddScoped<IGetFileUseCase, GetFileUseCase>();

var dataDir = Environment.GetEnvironmentVariable("STORAGE_DATA_DIR") ?? "/data";
builder.Services.AddInfrastructure(dataDir);
        
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/files", async (HttpRequest request, IStoreFileUseCase useCase, CancellationToken ct) =>
{
    if (!request.HasFormContentType)
    {
        return Results.BadRequest(new { error = "Expected multipart/form-data" });
    }

    var form = await request.ReadFormAsync(ct);
    var file = form.Files.GetFile("file");
    if (file is null || file.Length == 0)
    {
        return Results.BadRequest(new { error = "File is required (field name: file)" });
    }

    await using var stream = file.OpenReadStream();
    var resp = await useCase.ExecuteAsync(stream, file.FileName, ct);

    return Results.Ok(resp);
}).DisableAntiforgery();

app.MapGet("/files/{fileId}", async (string fileId, IGetFileUseCase useCase, CancellationToken ct) =>
{
    var result = await useCase.ExecuteAsync(fileId, ct);
    if (result is null)
    {
        return Results.NotFound(new { error = "File not found" });
    }
    return Results.File(result.Value.Stream, "application/octet-stream", result.Value.FileName);
});

app.Run();
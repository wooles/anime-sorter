using MalProxy.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<IMyAnimeListService, MyAnimeListService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(15);
});

// Configure CORS for static SPA frontends (e.g. sort.moe / GitHub Pages)
builder.Services.AddCors(options =>
{
    options.AddPolicy(""AllowAll"", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors(""AllowAll"");

// Routes
app.MapGet(""/"", () => Results.Ok(new
{
    Name = ""sort.moe MAL Proxy API"",
    Version = ""1.0.0"",
    Status = ""Online"",
    Endpoints = new[]
    {
        ""GET /health"",
        ""GET /api/mal/watchlist/{username}""
    }
}));

app.MapGet(""/health"", () => Results.Ok(new
{
    status = ""Healthy"",
    timestamp = DateTime.UtcNow
}));

app.MapGet(""/api/mal/watchlist/{username}"", async (
    string username,
    [FromServices] IMyAnimeListService malService,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(username))
    {
        return Results.BadRequest(new { error = ""Username cannot be empty."" });
    }

    try
    {
        var items = await malService.GetUserWatchlistAsync(username, cancellationToken);
        return Results.Ok(items);
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
    catch (HttpRequestException ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: StatusCodes.Status502BadGateway,
            title: ""External API Error"");
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Internal Server Error");
    }
});

app.MapGet("/api/mal/search", async (
    [FromQuery] string q,
    [FromServices] IMyAnimeListService malService,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(q))
    {
        return Results.BadRequest(new { error = "Query parameter 'q' cannot be empty." });
    }

    try
    {
        var items = await malService.SearchAnimeAsync(q, cancellationToken);
        return Results.Ok(items);
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Search Error");
    }
});

app.Run();

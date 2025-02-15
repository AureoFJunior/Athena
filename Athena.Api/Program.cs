using Athena.Domain.Repositories;
using Athena.Infrastructure.Data;
using Athena.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDataEntryRepository, DataEntryRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("VueApp", policy =>
    {
        policy.WithOrigins("http://localhost:8080")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("VueApp");

// Get entries by filter
app.MapGet("/dataentries", async (
    [FromQuery] string? category,
    [FromQuery] string? tag,
    [FromQuery] string? orderBy,
    [FromQuery] int pageSize,
    [FromQuery] int pageNumber,
    [FromQuery] bool descending,
    IDataEntryRepository repo) =>
{
    (var entries, var totalItems) = await repo.GetAllAsync(category, tag, orderBy, pageSize, pageNumber, descending);
    return Results.Ok(new { entries, totalItems});
})
.WithName("GetAllDataEntries")
.WithOpenApi();

// Get entry by id
app.MapGet("/dataentries/{id}", async (Guid id, IDataEntryRepository repo) =>
{
    var entry = await repo.GetByIdAsync(id);
    return entry is null ? Results.NotFound() : Results.Ok(entry);
})
.WithName("GetDataEntryById")
.WithOpenApi();

app.Run();

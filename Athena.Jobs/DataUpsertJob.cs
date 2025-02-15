using Athena.Domain.Entities;
using Athena.Domain.Models.DTO;
using Athena.Domain.Repositories;
using System.Text.Json;
using System.Collections.Concurrent;

namespace Athena.Jobs;

public class DataUpsertJob
{
    private readonly IDataEntryRepository _repository;
    private readonly HttpClient _httpClient;

    public DataUpsertJob(IDataEntryRepository repository, HttpClient httpClient)
    {
        _repository = repository;
        _httpClient = httpClient;
    }

    public async Task ExecuteAsync()
    {
        try
        {
            string url = BuildUrl();
            RootResponse? rootResponse = await RequestDataFrom(url);

            if (rootResponse == null || rootResponse.Images == null || rootResponse.Status != 200)
                return;

            var tasks = new ConcurrentBag<Task>();

            foreach (var item in rootResponse.Images)
                tasks.Add(UpsertDataAsync(item));

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private async Task<RootResponse?> RequestDataFrom(string url)
    {
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var rootResponse = JsonSerializer.Deserialize<RootResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return rootResponse;
    }

    private static string BuildUrl()
    {
        var category = "sfw";
        var count = 10;
        var additionalTags = "uniform";
        var blacklistedTags = "sad";

        var url = $"https://api.nekosia.cat/api/v1/images/{category}?count={count}" +
                  (!string.IsNullOrEmpty(additionalTags) ? $"&additionalTags={additionalTags}" : "") +
                  (!string.IsNullOrEmpty(blacklistedTags) ? $"&blacklistedTags={blacklistedTags}" : "");
        return url;
    }

    private async Task UpsertDataAsync(ImageResponse item)
    {
        float ratingValue = 0;
        ratingValue = GetRating(item);

        var dataEntry = new DataEntry(
            title: item.Category,
            description: string.Join(", ", item.Tags)
        )
        {
            ImageUrl = item.Image.Original.Url ?? "",
            Category = item.Category ?? "",
            Tags = item.Tags,
            Rating = ratingValue
        };

        await _repository.AddAsync(dataEntry);
    }

    private static float GetRating(ImageResponse item)
    {
        float ratingValue;
        if (float.TryParse(item.Rating, out float parsedRating))
        {
            ratingValue = parsedRating;
        }
        else
        {
            ratingValue = item.Rating.ToLower() switch
            {
                "safe" => 1.0f,
                "questionable" => 2.0f,
                "explicit" => 3.0f,
                _ => 0.0f
            };
        }

        return ratingValue;
    }
}

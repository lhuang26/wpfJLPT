using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WpfJLPT;

public sealed record JlptCategory(int Id, string Name);

public sealed record JlptEntry(
    int Id,
    string Entry,
    string Meaning,
    string Hiragana,
    string Katakana,
    string Positive,
    string Negative,
    string PastPositive,
    string PastNegative);

public sealed record JlptEntryData(
    string Entry,
    string Meaning,
    string Hiragana,
    string Katakana,
    string Positive,
    string Negative,
    string PastPositive,
    string PastNegative,
    IReadOnlyCollection<int> CategoryIds);

public sealed class JlptApiClient
{
    private static readonly HttpClient HttpClient = new()
    {
        BaseAddress = new Uri("https://apitekera-c8atgfakc0ggb0fw.westus3-01.azurewebsites.net/")
    };

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyList<JlptCategory>> GetCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        return await GetRequiredJsonAsync<List<JlptCategory>>(
            "api/categories",
            cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<JlptEntry>> GetEntriesAsync(
        int categoryId,
        CancellationToken cancellationToken = default)
    {
        return await GetRequiredJsonAsync<List<JlptEntry>>(
            $"api/categories/{categoryId}/entries",
            cancellationToken).ConfigureAwait(false);
    }

    public async Task OrderEntryAsync(
        int entryId,
        int orderType,
        CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.PostAsJsonAsync(
            $"api/entries/{entryId}/order",
            new OrderEntryRequest(orderType),
            JsonOptions,
            cancellationToken).ConfigureAwait(false);

        await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task SaveEntryAsync(
        JlptEntryData entry,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entry);

        using var response = await HttpClient.PostAsJsonAsync(
            "api/entries",
            entry,
            JsonOptions,
            cancellationToken).ConfigureAwait(false);

        await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<T> GetRequiredJsonAsync<T>(
        string requestUri,
        CancellationToken cancellationToken)
    {
        using var response = await HttpClient.GetAsync(
            requestUri,
            cancellationToken).ConfigureAwait(false);

        await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);

        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException($"The API returned an empty response for '{requestUri}'.");
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken)
            .ConfigureAwait(false);
        var details = string.IsNullOrWhiteSpace(responseBody)
            ? response.ReasonPhrase
            : responseBody;

        throw new HttpRequestException(
            $"ApiTekera returned {(int)response.StatusCode} ({response.StatusCode}): {details}",
            null,
            response.StatusCode);
    }

    private sealed record OrderEntryRequest(int OrderType);
}

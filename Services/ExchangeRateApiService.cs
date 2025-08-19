namespace PersonalAccountant.Services;

using System.Net.Http.Json;
using PersonalAccountant.Shared;

public class ExchangeRateApiService : ICurrencyService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string? _apiKey;

    public ExchangeRateApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _apiKey = _configuration["ExchangeRateApiKey"];
    }

    public async Task<decimal> GetExchangeRateAsync(string baseCurrency, string targetCurrency)
    {
        if (string.IsNullOrEmpty(_apiKey) || _apiKey == "YOUR_NEW_API_KEY_HERE")
        {
            Console.WriteLine("API key is missing or is a placeholder. Halting API call."); // Add this line
            return 0;
        }

        var requestUrl = $"https://v6.exchangerate-api.com/v6/{_apiKey}/latest/{baseCurrency}";

        try
        {
            // 1. Get the raw JSON response as a string
            var jsonResponse = await _httpClient.GetStringAsync(requestUrl);

            // 2. Log the raw JSON to the browser's console
            Console.WriteLine("API Response: " + jsonResponse);

            // 3. Try to parse the JSON
            var response = System.Text.Json.JsonSerializer.Deserialize<ExchangeRateApiResponse>(jsonResponse);

            if (response != null && response.Result == "success" &&
                response.ConversionRates.TryGetValue(targetCurrency, out var rate))
            {
                return rate;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching exchange rate: {ex.Message}");
        }

        return 0;
    }
}
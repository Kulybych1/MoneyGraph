using System.Text.Json.Serialization;

namespace PersonalAccountant.Shared;

// This structure matches the JSON response from ExchangeRate-API.com
public class ExchangeRateApiResponse
{
    [JsonPropertyName("result")]
    public string Result { get; set; }

    [JsonPropertyName("base_code")]
    public string BaseCode { get; set; }

    [JsonPropertyName("conversion_rates")]
    public Dictionary<string, decimal> ConversionRates { get; set; }
}
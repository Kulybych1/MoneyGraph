namespace PersonalAccountant.Services;

public interface ICurrencyService
{
    Task<decimal> GetExchangeRateAsync(string baseCurrency, string targetCurrency);
}
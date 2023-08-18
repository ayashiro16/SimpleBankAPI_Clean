using ICurrencyRate = SimpleBankAPI.Application.Interfaces.ICurrencyRate;

namespace SimpleBankAPI.UnitTests.Helpers;

public class FakeCurrencyClient : ICurrencyRate
{
    public async Task<Dictionary<string, decimal>> GetConversionRates(string? currencyCode)
    {
        var currencyList = currencyCode.Split(",");
        await Task.CompletedTask;
        return currencyList.ToDictionary(code => code, code => (decimal)0.80);
    }
}
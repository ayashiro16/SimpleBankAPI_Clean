using System.Net;
using Newtonsoft.Json;
using ICurrencyRate = SimpleBankAPI.Application.Interfaces.ICurrencyRate;

namespace SimpleBankAPI.Infrastructure.Clients;

public class CurrencyClient : ICurrencyRate
{
    private readonly HttpClient _currencyClient;

    public CurrencyClient(HttpClient currencyClient)
    {
        _currencyClient = currencyClient;
    }

    public async Task<Dictionary<string, decimal>> GetConversionRates(string? currencyCode)
    {
        var address = new UriBuilder(_currencyClient.BaseAddress!);
        address.Query += $"&currencies={currencyCode}";
        var response = await _currencyClient.GetAsync(address.ToString());
        if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
        {
            throw new ArgumentException("Could not process the provided currency code(s)");
        }
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, decimal>>>(responseBody);
        var rates = data?.GetValueOrDefault("data");

        return rates ?? new Dictionary<string, decimal>();
    }
}
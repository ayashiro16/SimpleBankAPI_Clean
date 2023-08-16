namespace SimpleBankAPI.Application.Interfaces;

public interface ICurrencyRate
{
    Task<Dictionary<string, decimal>> GetConversionRates(string? currencyCode);
}
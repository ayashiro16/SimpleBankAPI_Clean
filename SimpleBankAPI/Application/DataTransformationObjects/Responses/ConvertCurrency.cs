namespace SimpleBankAPI.Application.DataTransformationObjects.Responses;

public record ConvertCurrency(string? CurrencyCode, decimal? ConvertedBalance);
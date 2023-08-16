namespace SimpleBankAPI.Application.Validators;

public class CurrencyCode : Interfaces.IValidator
{
    public bool Validate(object? currencyCodes)
    {
        if (currencyCodes is null)
        {
            return true;
        }
        var codes = ((string)currencyCodes).Trim().ToUpper();
        if (codes.Length == 0)
        {
            return true;
        }
        if (!codes.All(c => char.IsLetter(c) || c == ','))
        {
            throw new ArgumentException("Cannot include numbers or special characters in currency codes. " +
                                        "Please enter 3-letter currency codes separated by commas if entering multiple codes.");
        }
        var allCodes = codes.Split(",");
        if (allCodes.Any(code => code.Length != 3))
        {
            throw new ArgumentException("Currency codes must be 3 letters long. " +
                                        "Please enter 3-letter currency codes separated by commas if entering multiple codes.");
        }
        
        return true;
    }
}
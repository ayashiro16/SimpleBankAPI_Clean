namespace SimpleBankAPI.Application.Validators;

public class SufficientFunds : Interfaces.IValidator
{
    public bool Validate(object argument)
    {
        var (balance, amount) = ((decimal, decimal))argument;
        if (balance < amount)
        {
            throw new InvalidOperationException("Insufficient funds");
        }

        return true;
    }
}
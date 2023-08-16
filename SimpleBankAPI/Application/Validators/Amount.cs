namespace SimpleBankAPI.Application.Validators;

public class Amount : Interfaces.IValidator
{
    public bool Validate(object amount)
    {
        if ((decimal)amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Cannot give a negative amount");
        }

        return true;
    }
}
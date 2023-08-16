namespace SimpleBankAPI.Application.Validators;

public class Username: Interfaces.IValidator
{
    public bool Validate(object name)
    {
        if (string.IsNullOrEmpty((string)name) || string.IsNullOrWhiteSpace((string)name))
        {
            throw new ArgumentException("Name field cannot be empty or white space");
        }
        if (!((string)name).All(x => char.IsWhiteSpace(x) || char.IsLetter(x)))
        {
            throw new ArgumentException("Name cannot contain special characters or numbers");
        }

        return true;
    }
}
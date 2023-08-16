namespace SimpleBankAPI.Application.Validators.Interfaces;

public interface IValidator
{
    bool Validate(object argument);
}
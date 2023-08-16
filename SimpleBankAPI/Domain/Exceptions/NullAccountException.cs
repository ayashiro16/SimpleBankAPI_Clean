namespace SimpleBankAPI.Domain.Exceptions;

public class NullAccountException : Exception
{
    public NullAccountException()
        : base("The account you are trying to access does not exist") {}
}
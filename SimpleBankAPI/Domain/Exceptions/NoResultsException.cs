namespace SimpleBankAPI.Domain.Exceptions;

public class NoResultsException : Exception
{
    public NoResultsException()
        : base("No accounts matched the provided search/filter criteria.") {}
}
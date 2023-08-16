using GetAccountsQuery = SimpleBankAPI.Application.DataTransformationObjects.Requests.GetAccountsQuery;

namespace SimpleBankAPI.Application.Validators;

public class Query : Interfaces.IValidator
{
    public bool Validate(object query)
    {
        var searchTerm = ((GetAccountsQuery)query).SearchTerm;
        var filterTerm = ((GetAccountsQuery)query).FilterTerm;
        if ((searchTerm is not null && !searchTerm.All(x => char.IsWhiteSpace(x) || char.IsLetter(x))) ||
                (filterTerm is not null && !filterTerm.All(x => char.IsWhiteSpace(x) || char.IsLetter(x))))
        {
            throw new ArgumentException("Search and filter terms cannot contain special characters or numbers");
        }
        var currentPage = ((GetAccountsQuery)query).CurrentPage;
        var pageSize = ((GetAccountsQuery)query).PageSize;
        if (currentPage < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(currentPage), "Page number must be greater than 0");
        }
        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than 0");
        }
        var sortBy = ((GetAccountsQuery)query).SortBy?.ToUpper().Trim();
        var order = ((GetAccountsQuery)query).SortOrder?.ToUpper().Trim();
        if (!string.IsNullOrEmpty(sortBy) && sortBy is not ("NAME" or "BALANCE"))
        {
            throw new ArgumentException("Must sort by one of the allowed values or leave the field empty.");
        }
        if (!string.IsNullOrEmpty(order) && order is not ("ASC" or "DESC"))
        {
            throw new ArgumentException("Must order by one of the allowed values or leave the field empty.");
        }

        return true;
    }
}
using SimpleBankAPI.Domain.Entities;
using SimpleBankAPI.Domain.Exceptions;
using GetAccountsQuery = SimpleBankAPI.Application.DataTransformationObjects.Requests.GetAccountsQuery;
using AccountContext = SimpleBankAPI.Infrastructure.Data.AccountContext;

namespace SimpleBankAPI.Infrastructure.Repositories;

public class AccountsRepository : Domain.Interfaces.IAccountsRepository
{
    private readonly AccountContext _context;

    public AccountsRepository(AccountContext context)
    {
        _context = context;
    }
    
    public ValueTask<Account?> Get(Guid id)
    {
        return _context.FindAsync(id);
    }

    public (IEnumerable<Account>, PaginationMetadata) GetAll(GetAccountsQuery query)
    {
        var collection = _context.GetAll();
        if (!string.IsNullOrEmpty(query.FilterTerm))
        {
            collection = collection.Where(account => account.Name.ToUpper().Contains(query.FilterTerm.Trim().ToUpper()));
        }
        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            collection = collection.Where(account => string.Equals(account.Name.ToUpper(), query.SearchTerm.Trim().ToUpper()));
        }
        if (!collection.Any())
        {
            throw new NoResultsException();
        }
        collection = query.SortBy?.ToUpper().Trim() switch
        {
            ("BALANCE") => collection.OrderBy(account => account.Balance),
            ("NAME") => collection.OrderBy(account => account.Name),
            _ => collection.OrderBy(account => account.Id)
        };
        if (query.SortOrder?.ToUpper().Trim() == "DESC")
        {
            collection = collection.Reverse();
        }
        var paginationMetadata = new PaginationMetadata(collection.Count(), query.PageSize, query.CurrentPage);
        var currentPage =  query.CurrentPage;
        var result = collection.Skip(query.PageSize * (currentPage - 1)).Take(query.PageSize).ToList();

        return (result, paginationMetadata);
    }

    public void Add(Account account)
    {
        _context.Add(account);
        _context.SaveChangesAsync();
    }

    public void Update(Account account, decimal amount)
    {
        account.Balance += amount;
        _context.SaveChangesAsync();
    }
}
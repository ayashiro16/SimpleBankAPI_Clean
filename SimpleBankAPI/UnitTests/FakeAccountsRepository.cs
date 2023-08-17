using SimpleBankAPI.Application.DataTransformationObjects.Requests;
using SimpleBankAPI.Domain.Entities;
using SimpleBankAPI.Domain.Interfaces;

namespace SimpleBankAPI.UnitTests;

public class FakeAccountsRepository : IAccountsRepository
{
    private readonly List<Account> _fakeAccountsContext;

    public FakeAccountsRepository()
    {
        _fakeAccountsContext = new List<Account>()
        {
            new Account()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000000"),
                Name = "Greg Jones",
                Balance = 222
            },
            new Account()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                Name = "Tom Hanks",
                Balance = 5000
            }, 
            new Account()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                Name = "Betty Crocker",
                Balance = 1000000
            }
        };
    }

    public ValueTask<Account?> Get(Guid id)
    {
        return new ValueTask<Account?>(_fakeAccountsContext.FirstOrDefault(user => user.Id == id));
    }

    public (List<Account>, PaginationMetadata) GetAll(GetAccountsQuery query)
    {
        return (_fakeAccountsContext, new PaginationMetadata(_fakeAccountsContext.Count, query.PageSize, query.CurrentPage));
    }

    public void Add(Account account)
    {
        _fakeAccountsContext.Add(account);
    }

    public void Update(Account account, decimal amount)
    {
        account.Balance += amount;
    }

    public int GetCount()
    {
        return _fakeAccountsContext.Count;
    }
}
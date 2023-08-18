using PaginationMetadata = SimpleBankAPI.Domain.Entities.PaginationMetadata;
using GetAccountsQuery = SimpleBankAPI.Application.DataTransformationObjects.Requests.GetAccountsQuery;
using Account = SimpleBankAPI.Domain.Entities.Account;

namespace SimpleBankAPI.Domain.Interfaces;

public interface IAccountsRepository
{
    ValueTask<Account?> Get(Guid id);
    (IEnumerable<Account>, PaginationMetadata) GetAll(GetAccountsQuery query);
    void Add(Account account);
    void Update(Account account, decimal amount);
}
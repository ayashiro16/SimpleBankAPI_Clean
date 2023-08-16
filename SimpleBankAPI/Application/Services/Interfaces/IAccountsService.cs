using SimpleBankAPI.Application.DataTransformationObjects.Responses;
using Account = SimpleBankAPI.Domain.Entities.Account;
using GetAccountsQuery = SimpleBankAPI.Application.DataTransformationObjects.Requests.GetAccountsQuery;
using PaginationMetadata = SimpleBankAPI.Domain.Entities.PaginationMetadata;

namespace SimpleBankAPI.Application.Services.Interfaces;

public interface IAccountsService
{
    Task<Account> CreateAccount(string name);
    ValueTask<Account> FindAccount(Guid id);
    (IEnumerable<Account>, PaginationMetadata) GetAllAccounts(GetAccountsQuery query);
    Task<Account> DepositFunds(Guid id, decimal amount);
    Task<Account> WithdrawFunds(Guid id, decimal amount);
    Task<Transfer> TransferFunds(Guid senderId, Guid recipientId, decimal amount);
    Task<IEnumerable<ConvertCurrency>> GetConvertedCurrency(Guid id, string currencyCode);
}
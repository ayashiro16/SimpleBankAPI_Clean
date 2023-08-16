using Microsoft.EntityFrameworkCore;
using Account = SimpleBankAPI.Domain.Entities.Account;

namespace SimpleBankAPI.Infrastructure.Data;

public class AccountContext : DbContext
{
    public DbSet<Account> Accounts { private get; init; }
    
    public AccountContext(DbContextOptions<AccountContext> options) : base(options) {}

    public void Add(Account account) => Accounts.Add(account);

    public ValueTask<Account?> FindAsync(Guid id) => Accounts.FindAsync(id);

    public IQueryable<Account> GetAll() => Accounts.AsQueryable();
}
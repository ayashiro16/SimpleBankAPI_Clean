using Xunit;
using SimpleBankAPI.Application.Interfaces;
using AccountNotFoundException = SimpleBankAPI.Domain.Exceptions.AccountNotFoundException;
using ValidatorFactory = SimpleBankAPI.API.Factories.ValidatorFactory;
using AccountsService = SimpleBankAPI.Application.Services.AccountsService;
using IValidator = SimpleBankAPI.Application.Validators.Interfaces.IValidator;
using FakeCurrencyClient = SimpleBankAPI.UnitTests.Helpers.FakeCurrencyClient;
using FakeAccountsRepository = SimpleBankAPI.UnitTests.Repositories.FakeAccountsRepository;

namespace SimpleBankAPI.UnitTests.AccountsServiceTests;

public class FindAccountTests
{
    private readonly AccountsService _systemUnderTest;

    public FindAccountTests()
    {
        var fakeAccountsRepository = new FakeAccountsRepository();
        var fakeCurrencyClient = new FakeCurrencyClient();
        IFactory<IValidator?> validators = new ValidatorFactory();
        _systemUnderTest = new AccountsService(fakeAccountsRepository, fakeCurrencyClient, validators);
    }
    
    [Fact]
    public async Task FindAccount_ValidAccountGuid_RetrievesAccount()
    {
        const string expectedAccountName = "Greg Jones";
        var stubAccountId = Guid.Parse("00000000-0000-0000-0000-000000000000");
        
        var account = await _systemUnderTest.FindAccount(stubAccountId);

        Assert.NotNull(account);
        Assert.Equal(expectedAccountName, account.Name);
    }

    [Fact]
    public async Task FindAccount_ValidGuidNoAccount_ThrowsAccountNotFoundException()
    {
        var stubInvalidId = Guid.Parse("00000000-0000-0000-0000-000000000009");
        
        var act = async () => await _systemUnderTest.FindAccount(stubInvalidId);

        var exception = await Assert.ThrowsAsync<AccountNotFoundException>(act);
        Assert.Equal("Could not find an account with the provided ID", exception.Message);
    }
}
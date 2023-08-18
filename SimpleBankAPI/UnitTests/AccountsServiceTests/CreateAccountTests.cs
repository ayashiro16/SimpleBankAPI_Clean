using Xunit;
using SimpleBankAPI.Application.Interfaces;
using ValidatorFactory = SimpleBankAPI.API.Factories.ValidatorFactory;
using AccountsService = SimpleBankAPI.Application.Services.AccountsService;
using IValidator = SimpleBankAPI.Application.Validators.Interfaces.IValidator;
using FakeCurrencyClient = SimpleBankAPI.UnitTests.Helpers.FakeCurrencyClient;
using FakeAccountsRepository = SimpleBankAPI.UnitTests.Repositories.FakeAccountsRepository;

namespace SimpleBankAPI.UnitTests.AccountsServiceTests;

public class CreateAccountTests
{
    private readonly AccountsService _systemUnderTest;
    private readonly FakeAccountsRepository _fakeAccountsRepository;

    public CreateAccountTests()
    {
        _fakeAccountsRepository = new FakeAccountsRepository();
        var fakeCurrencyClient = new FakeCurrencyClient();
        IFactory<IValidator?> validators = new ValidatorFactory();
        _systemUnderTest = new AccountsService(_fakeAccountsRepository, fakeCurrencyClient, validators);
    }

    [Fact]
    public async Task CreateAccount_ValidName_AddsAccountToRepository()
    {
        const string stubAccountName = "Valid Name";
        var countBeforeCreateAccount = _fakeAccountsRepository.GetCount();

        var account = await _systemUnderTest.CreateAccount(stubAccountName);

        Assert.NotNull(account);
        Assert.Equal(_fakeAccountsRepository.GetCount(), countBeforeCreateAccount + 1);
    }

    [Fact]
    public async Task CreateAccount_NullOrEmptyString_ThrowsArgumentException()
    {
        var act = () => _systemUnderTest.CreateAccount(string.Empty);

        var exception = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Equal("Name field cannot be empty or white space", exception.Message);
    }

    [Fact]
    public async Task CreateAccount_NumbersOrSpecialCharacters_ThrowsArgumentException()
    {
        const string stubInvalidAccountName = "Name123!";
        
        var act = () => _systemUnderTest.CreateAccount(stubInvalidAccountName);

        var exception = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Equal("Name cannot contain special characters or numbers", exception.Message);
    }
}
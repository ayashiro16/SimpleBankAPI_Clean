using Xunit;
using SimpleBankAPI.Application.Interfaces;
using ValidatorFactory = SimpleBankAPI.API.Factories.ValidatorFactory;
using AccountsService = SimpleBankAPI.Application.Services.AccountsService;
using IValidator = SimpleBankAPI.Application.Validators.Interfaces.IValidator;
using FakeCurrencyClient = SimpleBankAPI.UnitTests.Helpers.FakeCurrencyClient;
using FakeAccountsRepository = SimpleBankAPI.UnitTests.Repositories.FakeAccountsRepository;
using NullAccountException = SimpleBankAPI.Domain.Exceptions.NullAccountException;
using ConvertCurrency = SimpleBankAPI.Application.DataTransformationObjects.Responses.ConvertCurrency;

namespace SimpleBankAPI.UnitTests.AccountsServiceTests;

public class GetConvertedCurrencyTests
{
    private readonly AccountsService _systemUnderTest;

    public GetConvertedCurrencyTests()
    {
        var fakeAccountsRepository = new FakeAccountsRepository();
        var fakeCurrencyClient = new FakeCurrencyClient();
        IFactory<IValidator?> validators = new ValidatorFactory();
        _systemUnderTest = new AccountsService(fakeAccountsRepository, fakeCurrencyClient, validators);
    }
    
    [Fact]
    public async Task GetConvertedCurrency_ValidParameters_ReturnsConvertedBalances()
    {
        const string stubCurrencyCodes = "eur,jpy,cad";
        var accountBalance = (await _systemUnderTest.FindAccount(Guid.Parse("00000000-0000-0000-0000-000000000000"))).Balance;
        var expectedConvertedBalance = new List<ConvertCurrency>()
        {
            new ConvertCurrency("EUR", (decimal)0.80 * accountBalance),
            new ConvertCurrency("JPY", (decimal)0.80 * accountBalance),
            new ConvertCurrency("CAD", (decimal)0.80 * accountBalance)
        };

        var convertedBalances =
            await _systemUnderTest.GetConvertedCurrency(Guid.Parse("00000000-0000-0000-0000-000000000000"), stubCurrencyCodes);

        Assert.Equal(expectedConvertedBalance, convertedBalances);
    }
    
    [Fact]
    public async Task GetConvertedCurrency_InvalidCurrencyCodes_ThrowsArgumentException()
    {
        const string stubInvalidCurrencyCodes = "invalid,c0de$";
        var stubAccountId = Guid.Parse("00000000-0000-0000-0000-000000000000");

        var act = async () => await _systemUnderTest.GetConvertedCurrency(stubAccountId, stubInvalidCurrencyCodes);
        
        var exception = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Equal("Cannot include numbers or special characters in currency codes. " +
                     "Please enter 3-letter currency codes separated by commas if entering multiple codes.", 
            exception.Message);
    }
    
    [Fact]
    public async Task GetConvertedCurrency_NonexistentAccount_ThrowsNullAccountException()
    {
        const string stubCurrencyCodes = "eur,jpy,cad";
        var stubInvalidAccountId = Guid.Parse("00000000-0000-0000-0000-000000000009");

        var act = async () => await _systemUnderTest.GetConvertedCurrency(stubInvalidAccountId, stubCurrencyCodes);
        
        var exception = await Assert.ThrowsAsync<NullAccountException>(act);
        Assert.Equal("The account you are trying to access does not exist", exception.Message);
    }
}
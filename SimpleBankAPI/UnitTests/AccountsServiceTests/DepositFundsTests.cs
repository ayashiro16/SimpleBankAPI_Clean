using Xunit;
using SimpleBankAPI.Application.Interfaces;
using ValidatorFactory = SimpleBankAPI.API.Factories.ValidatorFactory;
using AccountsService = SimpleBankAPI.Application.Services.AccountsService;
using IValidator = SimpleBankAPI.Application.Validators.Interfaces.IValidator;
using FakeCurrencyClient = SimpleBankAPI.UnitTests.Helpers.FakeCurrencyClient;
using FakeAccountsRepository = SimpleBankAPI.UnitTests.Repositories.FakeAccountsRepository;

namespace SimpleBankAPI.UnitTests.AccountsServiceTests;

public class DepositFundsTests
{
    private readonly AccountsService _systemUnderTest;

    public DepositFundsTests()
    {
        var fakeAccountsRepository = new FakeAccountsRepository();
        var fakeCurrencyClient = new FakeCurrencyClient();
        IFactory<IValidator?> validators = new ValidatorFactory();
        _systemUnderTest = new AccountsService(fakeAccountsRepository, fakeCurrencyClient, validators);
    }
    
    [Fact]
    public async Task DepositFunds_ValidParameters_AddsToAccountBalance()
    {
        const decimal stubAmount = 100;
        var stubAccountId = Guid.Parse("00000000-0000-0000-0000-000000000000");

        var balanceBeforeDeposit = (await _systemUnderTest.FindAccount(stubAccountId)).Balance;
        await _systemUnderTest.DepositFunds(stubAccountId, stubAmount);
        var balanceAfterDeposit = (await _systemUnderTest.FindAccount(stubAccountId)).Balance;
        
        Assert.Equal(balanceBeforeDeposit + stubAmount, balanceAfterDeposit);
    }

    [Fact]
    public async Task DepositFunds_NegativeAmount_ThrowsArgumentOutOfRangeException()
    {
        const decimal stubNegativeAmount = -100;
        var stubAccountId = Guid.Parse("00000000-0000-0000-0000-000000000000");

        var balanceBeforeDeposit = (await _systemUnderTest.FindAccount(stubAccountId)).Balance;
        var act = async () => await _systemUnderTest.DepositFunds(stubAccountId, stubNegativeAmount);
        var balanceAfterDeposit = (await _systemUnderTest.FindAccount(stubAccountId)).Balance;

        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(act);
        Assert.Equal("Cannot give a negative amount (Parameter 'amount')", exception.Message);
        Assert.Equal(balanceBeforeDeposit, balanceAfterDeposit);
    }
}
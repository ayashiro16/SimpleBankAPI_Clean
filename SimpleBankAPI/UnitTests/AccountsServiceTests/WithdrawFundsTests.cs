using Xunit;
using SimpleBankAPI.Application.Interfaces;
using ValidatorFactory = SimpleBankAPI.API.Factories.ValidatorFactory;
using AccountsService = SimpleBankAPI.Application.Services.AccountsService;
using IValidator = SimpleBankAPI.Application.Validators.Interfaces.IValidator;
using FakeCurrencyClient = SimpleBankAPI.UnitTests.Helpers.FakeCurrencyClient;
using FakeAccountsRepository = SimpleBankAPI.UnitTests.Repositories.FakeAccountsRepository;

namespace SimpleBankAPI.UnitTests.AccountsServiceTests;

public class WithdrawFundsTests
{
    private readonly AccountsService _systemUnderTest;

    public WithdrawFundsTests()
    {
        var fakeAccountsRepository = new FakeAccountsRepository();
        var fakeCurrencyClient = new FakeCurrencyClient();
        IFactory<IValidator?> validators = new ValidatorFactory();
        _systemUnderTest = new AccountsService(fakeAccountsRepository, fakeCurrencyClient, validators);
    }
    
    [Fact]
    public async Task WithdrawFunds_ValidParameters_SubtractsFromAccountBalance()
    {
        const decimal stubAmount = 100;
        var stubAccountId = Guid.Parse("00000000-0000-0000-0000-000000000000");
        
        var balanceBeforeWithdraw = (await _systemUnderTest.FindAccount(stubAccountId)).Balance;
        await _systemUnderTest.WithdrawFunds(stubAccountId, stubAmount);
        var balanceAfterWithdraw = (await _systemUnderTest.FindAccount(stubAccountId)).Balance;
        
        Assert.Equal(balanceBeforeWithdraw - stubAmount, balanceAfterWithdraw);
    }
    
    [Fact]
    public async Task WithdrawFunds_NegativeAmount_ThrowsArgumentOutOfRangeException()
    {
        const decimal stubNegativeAmount = -100;
        var stubAccountId = Guid.Parse("00000000-0000-0000-0000-000000000000");

        var balanceBeforeWithdraw = (await _systemUnderTest.FindAccount(stubAccountId)).Balance;
        var act = async () => await _systemUnderTest.WithdrawFunds(stubAccountId, stubNegativeAmount);
        var balanceAfterWithdraw = (await _systemUnderTest.FindAccount(stubAccountId)).Balance;

        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(act);
        Assert.Equal("Cannot give a negative amount (Parameter 'amount')", exception.Message);
        Assert.Equal(balanceBeforeWithdraw, balanceAfterWithdraw);
    }
    
    [Fact]
    public async Task WithdrawFunds_AmountGreaterThanBalance_ThrowsInvalidOperationException()
    {
        const decimal stubAmount = 10000000;
        var stubAccountId = Guid.Parse("00000000-0000-0000-0000-000000000000");

        var balanceBeforeWithdraw = (await _systemUnderTest.FindAccount(stubAccountId)).Balance;
        var act = async () => await _systemUnderTest.WithdrawFunds(stubAccountId, stubAmount);
        var balanceAfterWithdraw = (await _systemUnderTest.FindAccount(stubAccountId)).Balance;

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Equal("Insufficient funds", exception.Message);
        Assert.Equal(balanceBeforeWithdraw, balanceAfterWithdraw);
    }
}
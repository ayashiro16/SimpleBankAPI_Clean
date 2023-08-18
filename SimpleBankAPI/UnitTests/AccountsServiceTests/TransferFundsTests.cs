using Xunit;
using SimpleBankAPI.Application.Interfaces;
using ValidatorFactory = SimpleBankAPI.API.Factories.ValidatorFactory;
using AccountsService = SimpleBankAPI.Application.Services.AccountsService;
using IValidator = SimpleBankAPI.Application.Validators.Interfaces.IValidator;
using FakeCurrencyClient = SimpleBankAPI.UnitTests.Helpers.FakeCurrencyClient;
using FakeAccountsRepository = SimpleBankAPI.UnitTests.Repositories.FakeAccountsRepository;
using AccountNotFoundException = SimpleBankAPI.Domain.Exceptions.AccountNotFoundException;


namespace SimpleBankAPI.UnitTests.AccountsServiceTests;

public class TransferFundsTests
{
    private readonly AccountsService _systemUnderTest;

    public TransferFundsTests()
    {
        var fakeAccountsRepository = new FakeAccountsRepository();
        var fakeCurrencyClient = new FakeCurrencyClient();
        IFactory<IValidator?> validators = new ValidatorFactory();
        _systemUnderTest = new AccountsService(fakeAccountsRepository, fakeCurrencyClient, validators);
    }
    
    [Fact]
    public async Task TransferFunds_ValidParameters_TransfersFundsFromSenderToRecipient()
    {
        var stubSenderId = Guid.Parse("00000000-0000-0000-0000-000000000000");
        var stubRecipientId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        
        var senderBalanceBeforeTransfer = (await _systemUnderTest.FindAccount(stubSenderId)).Balance;
        var recipientBalanceBeforeTransfer = (await _systemUnderTest.FindAccount(stubRecipientId)).Balance;
        await _systemUnderTest.TransferFunds(stubSenderId, stubRecipientId, 100);
        var senderBalanceAfterTransfer = (await _systemUnderTest.FindAccount(stubSenderId)).Balance;
        var recipientBalanceAfterTransfer = (await _systemUnderTest.FindAccount(stubRecipientId)).Balance;
        
        Assert.Equal(senderBalanceBeforeTransfer - 100, senderBalanceAfterTransfer);
        Assert.Equal(recipientBalanceBeforeTransfer + 100, recipientBalanceAfterTransfer);
    }
    
    [Fact]
    public async Task TransferFunds_BothInvalidGuids_ThrowsAccountNotFoundException()
    {
        var stubSenderId = Guid.Parse("00000000-0000-0000-0000-000000000009");
        var stubRecipientId = Guid.Parse("00000000-0000-0000-0000-000000000010");

        var act = async () => await _systemUnderTest.TransferFunds(stubSenderId, stubRecipientId, 100);

        var exception = await Assert.ThrowsAsync<AccountNotFoundException>(act);
        Assert.Equal("Could not find the sender and recipient account(s)", exception.Message);
    }
    
    [Fact]
    public async Task TransferFunds_InvalidSenderGuid_ThrowsAccountNotFoundException()
    {
        var stubSenderId = Guid.Parse("00000000-0000-0000-0000-000000000009");
        var stubRecipientId = Guid.Parse("00000000-0000-0000-0000-000000000000");

        var act = async () => await _systemUnderTest.TransferFunds(stubSenderId, stubRecipientId, 100);

        var exception = await Assert.ThrowsAsync<AccountNotFoundException>(act);
        Assert.Equal("Could not find the sender account(s)", exception.Message);
    }
    
    [Fact]
    public async Task TransferFunds_InvalidRecipientGuid_ThrowsAccountNotFoundException()
    {
        var stubSenderId = Guid.Parse("00000000-0000-0000-0000-000000000000");
        var stubRecipientId = Guid.Parse("00000000-0000-0000-0000-000000000009");

        var act = async () => await _systemUnderTest.TransferFunds(stubSenderId, stubRecipientId, 100);

        var exception = await Assert.ThrowsAsync<AccountNotFoundException>(act);
        Assert.Equal("Could not find the recipient account(s)", exception.Message);
    }
   
    [Fact]
    public async Task TransferFunds_AmountGreaterThanSenderBalance_ThrowsInvalidOperationException()
    {
        var stubSenderId = Guid.Parse("00000000-0000-0000-0000-000000000000");
        var stubRecipientId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        var act = async () => await _systemUnderTest.TransferFunds(stubSenderId, stubRecipientId, 100000);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Equal("Insufficient funds", exception.Message);
    }
}
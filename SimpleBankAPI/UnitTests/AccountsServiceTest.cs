using SimpleBankAPI.API.Factories;
using SimpleBankAPI.Application.DataTransformationObjects.Responses;
using SimpleBankAPI.Application.Interfaces;
using SimpleBankAPI.Application.Services;
using SimpleBankAPI.Application.Validators.Interfaces;
using SimpleBankAPI.Domain.Exceptions;
using Xunit;

namespace SimpleBankAPI.UnitTests;

public class AccountsServiceTest
{
    private readonly FakeAccountsRepository _fakeAccountsRepository;
    private readonly FakeCurrencyClient _fakeCurrencyClient;
    private readonly IFactory<IValidator?> _validators;

    public AccountsServiceTest()
    {
        _fakeAccountsRepository = new FakeAccountsRepository();
        _fakeCurrencyClient = new FakeCurrencyClient();
        _validators = new ValidatorFactory();
    }

    [Fact]
    public async Task CreateAccount_ValidName_AddsAccountToRepository()
    {
        //Arrange
        var systemUnderTest = new AccountsService(_fakeAccountsRepository, _fakeCurrencyClient, _validators);
        var countBeforeCreateAccount = _fakeAccountsRepository.GetCount();

        //Act
        var account = await systemUnderTest.CreateAccount("Amy Winehouse");

        //Assert
        Assert.NotNull(account);
        Assert.Equal(_fakeAccountsRepository.GetCount(), countBeforeCreateAccount + 1);
    }

    [Fact]
    public async Task CreateAccount_NullOrEmptyString_ThrowsArgumentException()
    {
        //Arrange
        var systemUnderTest = new AccountsService(_fakeAccountsRepository, _fakeCurrencyClient, _validators);

        //Act
        var act = () => systemUnderTest.CreateAccount(string.Empty);

        //Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Equal("Name field cannot be empty or white space", exception.Message);
    }

    [Fact]
    public async Task CreateAccount_NumbersOrSpecialCharacters_ThrowsArgumentException()
    {
        //Arrange
        var systemUnderTest = new AccountsService(_fakeAccountsRepository, _fakeCurrencyClient, _validators);

        //Act
        var act = () => systemUnderTest.CreateAccount("Name123!");

        //Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Equal("Name cannot contain special characters or numbers", exception.Message);
    }

    [Fact]
    public async Task FindAccount_ValidAccountGuid_RetrievesAccount()
    {
        //Arrange
        var systemUnderTest = new AccountsService(_fakeAccountsRepository, _fakeCurrencyClient, _validators);

        //Act
        var account = await systemUnderTest.FindAccount(Guid.Parse("00000000-0000-0000-0000-000000000000"));

        //Assert
        Assert.NotNull(account);
        Assert.Equal("Greg Jones", account.Name);
    }

    [Fact]
    public async Task FindAccount_ValidGuidNoAccount_ThrowsAccountNotFoundException()
    {
        //Arrange
        var systemUnderTest = new AccountsService(_fakeAccountsRepository, _fakeCurrencyClient, _validators);

        //Act
        var act = async () => await systemUnderTest.FindAccount(Guid.Parse("00000000-0000-0000-0000-000000000009"));

        //Assert
        var exception = await Assert.ThrowsAsync<AccountNotFoundException>(act);
        Assert.Equal("Could not find an account with the provided ID", exception.Message);
    }

    //TODO add tests for GetAllAccounts

    [Fact]
    public async Task DepositFunds_ValidParameters_AddsToAccountBalance()
    {
        //Arrange
        var systemUnderTest = new AccountsService(_fakeAccountsRepository, _fakeCurrencyClient, _validators);
        var accountId = Guid.Parse("00000000-0000-0000-0000-000000000000");

        //Act
        var balanceBeforeDeposit = (await systemUnderTest.FindAccount(accountId)).Balance;
        await systemUnderTest.DepositFunds(accountId, 100);
        var balanceAfterDeposit = (await systemUnderTest.FindAccount(accountId)).Balance;
        
        //Assert
        Assert.Equal(balanceBeforeDeposit + 100, balanceAfterDeposit);
    }

    [Fact]
    public async Task DepositFunds_NegativeAmount_ThrowsArgumentOutOfRangeException()
    {
        //Arrange
        var systemUnderTest = new AccountsService(_fakeAccountsRepository, _fakeCurrencyClient, _validators);
        var accountId = Guid.Parse("00000000-0000-0000-0000-000000000000");

        //Act
        var balanceBeforeDeposit = (await systemUnderTest.FindAccount(accountId)).Balance;
        var act = async () => await systemUnderTest.DepositFunds(accountId, -100);
        var balanceAfterDeposit = (await systemUnderTest.FindAccount(accountId)).Balance;

        //Assert
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(act);
        Assert.Equal("Cannot give a negative amount (Parameter 'amount')", exception.Message);
        Assert.Equal(balanceBeforeDeposit, balanceAfterDeposit);
    }
    
    [Fact]
    public async Task WithdrawFunds_ValidParameters_SubtractsFromAccountBalance()
    {
        //Arrange
        var systemUnderTest = new AccountsService(_fakeAccountsRepository, _fakeCurrencyClient, _validators);
        var accountId = Guid.Parse("00000000-0000-0000-0000-000000000000");
        
        //Act
        var balanceBeforeWithdraw = (await systemUnderTest.FindAccount(accountId)).Balance;
        await systemUnderTest.WithdrawFunds(accountId, 100);
        var balanceAfterWithdraw = (await systemUnderTest.FindAccount(accountId)).Balance;
        
        //Assert
        Assert.Equal(balanceBeforeWithdraw - 100, balanceAfterWithdraw);
    }
    
    [Fact]
    public async Task WithdrawFunds_NegativeAmount_ThrowsArgumentOutOfRangeException()
    {
        //Arrange
        var systemUnderTest = new AccountsService(_fakeAccountsRepository, _fakeCurrencyClient, _validators);
        var accountId = Guid.Parse("00000000-0000-0000-0000-000000000000");

        //Act
        var balanceBeforeWithdraw = (await systemUnderTest.FindAccount(accountId)).Balance;
        var act = async () => await systemUnderTest.WithdrawFunds(accountId, -100);
        var balanceAfterWithdraw = (await systemUnderTest.FindAccount(accountId)).Balance;

        //Assert
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(act);
        Assert.Equal("Cannot give a negative amount (Parameter 'amount')", exception.Message);
        Assert.Equal(balanceBeforeWithdraw, balanceAfterWithdraw);
    }
    
    [Fact]
    public async Task WithdrawFunds_AmountGreaterThanBalance_ThrowsInvalidOperationException()
    {
        //Arrange
        var systemUnderTest = new AccountsService(_fakeAccountsRepository, _fakeCurrencyClient, _validators);
        var accountId = Guid.Parse("00000000-0000-0000-0000-000000000000");

        //Act
        var balanceBeforeWithdraw = (await systemUnderTest.FindAccount(accountId)).Balance;
        var act = async () => await systemUnderTest.WithdrawFunds(accountId, 10000000);
        var balanceAfterWithdraw = (await systemUnderTest.FindAccount(accountId)).Balance;

        //Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Equal("Insufficient funds", exception.Message);
        Assert.Equal(balanceBeforeWithdraw, balanceAfterWithdraw);
    }

    [Fact]
    public async Task TransferFunds_ValidParameters_TransfersFundsFromSenderToRecipient()
    {
        //Arrange
        var systemUnderTest = new AccountsService(_fakeAccountsRepository, _fakeCurrencyClient, _validators);
        var senderId = Guid.Parse("00000000-0000-0000-0000-000000000000");
        var recipientId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        
        //Act
        var senderBalanceBeforeTransfer = (await systemUnderTest.FindAccount(senderId)).Balance;
        var recipientBalanceBeforeTransfer = (await systemUnderTest.FindAccount(recipientId)).Balance;
        await systemUnderTest.TransferFunds(senderId, recipientId, 100);
        var senderBalanceAfterTransfer = (await systemUnderTest.FindAccount(senderId)).Balance;
        var recipientBalanceAfterTransfer = (await systemUnderTest.FindAccount(recipientId)).Balance;
        
        //Assert
        Assert.Equal(senderBalanceBeforeTransfer - 100, senderBalanceAfterTransfer);
        Assert.Equal(recipientBalanceBeforeTransfer + 100, recipientBalanceAfterTransfer);
    }
    
    [Fact]
    public async Task TransferFunds_BothInvalidGuids_ThrowsAccountNotFoundException()
    {
        //Arrange
        var systemUnderTest = new AccountsService(_fakeAccountsRepository, _fakeCurrencyClient, _validators);
        var senderId = Guid.Parse("00000000-0000-0000-0000-000000000009");
        var recipientId = Guid.Parse("00000000-0000-0000-0000-000000000010");

        //Act
        var act = async () => await systemUnderTest.TransferFunds(senderId, recipientId, 100);

        //Assert
        var exception = await Assert.ThrowsAsync<AccountNotFoundException>(act);
        Assert.Equal("Could not find the sender and recipient account(s)", exception.Message);
    }
    
    [Fact]
    public async Task TransferFunds_InvalidSenderGuid_ThrowsAccountNotFoundException()
    {
        //Arrange
        var systemUnderTest = new AccountsService(_fakeAccountsRepository, _fakeCurrencyClient, _validators);
        var senderId = Guid.Parse("00000000-0000-0000-0000-000000000009");
        var recipientId = Guid.Parse("00000000-0000-0000-0000-000000000000");

        //Act
        var act = async () => await systemUnderTest.TransferFunds(senderId, recipientId, 100);

        //Assert
        var exception = await Assert.ThrowsAsync<AccountNotFoundException>(act);
        Assert.Equal("Could not find the sender account(s)", exception.Message);
    }
    
    [Fact]
    public async Task TransferFunds_InvalidRecipientGuid_ThrowsAccountNotFoundException()
    {
        //Arrange
        var systemUnderTest = new AccountsService(_fakeAccountsRepository, _fakeCurrencyClient, _validators);
        var senderId = Guid.Parse("00000000-0000-0000-0000-000000000000");
        var recipientId = Guid.Parse("00000000-0000-0000-0000-000000000009");

        //Act
        var act = async () => await systemUnderTest.TransferFunds(senderId, recipientId, 100);

        //Assert
        var exception = await Assert.ThrowsAsync<AccountNotFoundException>(act);
        Assert.Equal("Could not find the recipient account(s)", exception.Message);
    }
   
    [Fact]
    public async Task TransferFunds_AmountGreaterThanSenderBalance_ThrowsInvalidOperationException()
    {
        //Arrange
        var systemUnderTest = new AccountsService(_fakeAccountsRepository, _fakeCurrencyClient, _validators);
        var senderId = Guid.Parse("00000000-0000-0000-0000-000000000000");
        var recipientId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        //Act
        var act = async () => await systemUnderTest.TransferFunds(senderId, recipientId, 100000);

        //Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Equal("Insufficient funds", exception.Message);
    }

    [Fact]
    public async Task GetConvertedCurrency_ValidParameters_ReturnsConvertedBalances()
    {
        //Arrange
        var systemUnderTest = new AccountsService(_fakeAccountsRepository, _fakeCurrencyClient, _validators);
        var accountBalance = (await systemUnderTest.FindAccount(Guid.Parse("00000000-0000-0000-0000-000000000000"))).Balance;
        var expectedConvertedBalance = new List<ConvertCurrency>()
        {
            new ConvertCurrency("EUR", (decimal)0.80 * accountBalance),
            new ConvertCurrency("JPY", (decimal)0.80 * accountBalance),
            new ConvertCurrency("CAD", (decimal)0.80 * accountBalance)
        };

        //Act
        var convertedBalances =
            await systemUnderTest.GetConvertedCurrency(Guid.Parse("00000000-0000-0000-0000-000000000000"), "eur,jpy,cad");

        //Assert
        Assert.Equal(expectedConvertedBalance, convertedBalances);
    }
    
    [Fact]
    public async Task GetConvertedCurrency_InvalidCurrencyCodes_ThrowsArgumentException()
    {
        //Arrange
        var systemUnderTest = new AccountsService(_fakeAccountsRepository, _fakeCurrencyClient, _validators);
        var accountId = Guid.Parse("00000000-0000-0000-0000-000000000000");

        //Act
        var act = async () => await systemUnderTest.GetConvertedCurrency(accountId, "invalid,c0de$");
        
        //Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Equal("Cannot include numbers or special characters in currency codes. " +
                     "Please enter 3-letter currency codes separated by commas if entering multiple codes.", 
            exception.Message);
    }
}
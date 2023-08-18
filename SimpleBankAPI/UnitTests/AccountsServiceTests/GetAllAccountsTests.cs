using Xunit;
using SimpleBankAPI.Domain.Entities;
using SimpleBankAPI.Application.Interfaces;
using ValidatorFactory = SimpleBankAPI.API.Factories.ValidatorFactory;
using AccountsService = SimpleBankAPI.Application.Services.AccountsService;
using IValidator = SimpleBankAPI.Application.Validators.Interfaces.IValidator;
using FakeCurrencyClient = SimpleBankAPI.UnitTests.Helpers.FakeCurrencyClient;
using FakeAccountsRepository = SimpleBankAPI.UnitTests.Repositories.FakeAccountsRepository;
using GetAccountsQuery = SimpleBankAPI.Application.DataTransformationObjects.Requests.GetAccountsQuery;

namespace SimpleBankAPI.UnitTests.AccountsServiceTests;

public class GetAllAccountsTests
{
    private readonly AccountsService _systemUnderTest;

    public GetAllAccountsTests()
    {
        var fakeAccountsRepository = new FakeAccountsRepository();
        var fakeCurrencyClient = new FakeCurrencyClient();
        IFactory<IValidator?> validators = new ValidatorFactory();
        _systemUnderTest = new AccountsService(fakeAccountsRepository, fakeCurrencyClient, validators);
    }
    
    [Fact]
    public async Task GetAllAccounts_EmptyQuery_ReturnsAllAccountsWithPaginationMetadata()
    {
        var stubQuery = new GetAccountsQuery();

        var allRetrievedAccounts = _systemUnderTest.GetAllAccounts(stubQuery);
        
        Assert.IsAssignableFrom<IEnumerable<Account>>(allRetrievedAccounts.Item1);
        Assert.IsAssignableFrom<PaginationMetadata>(allRetrievedAccounts.Item2);
        Assert.NotEmpty(allRetrievedAccounts.Item1);
    }
    
    [Fact]
    public async Task GetAllAccounts_InvalidSearchAndFilterTerms_ThrowsArgumentException()
    {
        const string stubInvalidSearchTerm = "se@rch";
        const string stubInvalidFilterTerm = "filter1";
        var stubQuery = new GetAccountsQuery()
        {
            SearchTerm = stubInvalidSearchTerm,
            FilterTerm = stubInvalidFilterTerm
        };

        var act = async () => _systemUnderTest.GetAllAccounts(stubQuery);
        
        var exception = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Equal("Search and filter terms cannot contain special characters or numbers", exception.Message);
    }
    
    [Fact]
    public async Task GetAllAccounts_NegativePageSize_ThrowsArgumentOutOfRangeException()
    {
        const int stubNegativePageSize = -1;
        var stubQuery = new GetAccountsQuery()
        {
            PageSize = stubNegativePageSize
        };

        var act = async () =>  _systemUnderTest.GetAllAccounts(stubQuery);
        
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(act);
        Assert.Equal("Page size must be greater than 0 (Parameter 'pageSize')", exception.Message);
    }
    
    [Fact]
    public async Task GetAllAccounts_NegativeCurrentPage_ThrowsArgumentOutOfRangeException()
    {
        const int stubNegativeCurrentPage = -1;

        var stubQuery = new GetAccountsQuery()
        {
            CurrentPage = stubNegativeCurrentPage
        };

        var act = async () =>  _systemUnderTest.GetAllAccounts(stubQuery);
        
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(act);
        Assert.Equal("Page number must be greater than 0 (Parameter 'currentPage')", exception.Message);
    }
    
    [Fact]
    public async Task GetAllAccounts_InvalidSortBy_ThrowsArgumentException()
    {
        const string stubInvalidSortBy = "Age";
        var stubQuery = new GetAccountsQuery()
        {
            SortBy = stubInvalidSortBy
        };

        var act = async () =>  _systemUnderTest.GetAllAccounts(stubQuery);
        
        var exception = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Equal("Must sort by one of the allowed values or leave the field empty.", exception.Message);
    }
    
    [Fact]
    public async Task GetAllAccounts_InvalidSortOrder_ThrowsArgumentException()
    {
        const string stubInvalidSortOrder = "UP";
        var stubQuery = new GetAccountsQuery()
        {
            SortOrder = stubInvalidSortOrder
        };

        var act = async () =>  _systemUnderTest.GetAllAccounts(stubQuery);
        
        var exception = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Equal("Must order by one of the allowed values or leave the field empty.", exception.Message);
    }
}
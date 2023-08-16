namespace SimpleBankAPI.Application.DataTransformationObjects.Requests;

public record GetAccountsQuery
{ 
    /// <summary>
    /// filter for names that contain this string
    /// </summary>
    public string? FilterTerm { get; init;  }
    
    /// <summary>
    /// search for names that match this string exactly
    /// </summary>
    public string? SearchTerm { get; init; }
    
    /// <summary>
    /// Allowed values:
    /// "Name"
    /// "Balance"
    /// </summary>
    public string? SortBy { get; init; }
    
    /// <summary>
    /// Allowed values:
    /// "ASC" - Ascending order
    /// "DESC" - Descending order
    /// </summary>
    public string? SortOrder { get; init; }

    public int CurrentPage { get; init; } = 1;

    public int PageSize { get; init; } = 10;
}
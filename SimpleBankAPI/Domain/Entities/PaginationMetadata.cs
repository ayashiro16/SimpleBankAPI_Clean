namespace SimpleBankAPI.Domain.Entities;

public record PaginationMetadata
{
    public int TotalItemCount { get; init; }
    public int TotalPageCount { get; init; }
    public int PageSize { get; init; }
    public int CurrentPage { get; init; }

    public PaginationMetadata(int totalItemCount, int pageSize, int currentPage)
    {
        TotalItemCount = totalItemCount;
        PageSize = pageSize;
        TotalPageCount = (int)Math.Ceiling(TotalItemCount/(double)PageSize);
        CurrentPage = currentPage > TotalPageCount ? TotalPageCount : currentPage;
    }
}
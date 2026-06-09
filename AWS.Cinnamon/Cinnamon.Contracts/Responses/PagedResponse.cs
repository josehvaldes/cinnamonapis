namespace Cinnamon.Contracts.Responses
{
    public record PagedResponse<T>(
        IReadOnlyList<T> Items,
        int TotalCount,
        int PageNumber,
        int PageSize,
        int TotalPages,
        bool HasNextPage,
        bool HasPreviousPage);
}

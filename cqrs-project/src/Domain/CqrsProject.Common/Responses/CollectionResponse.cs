namespace CqrsProject.Common.Responses;

public record CollectionResponse<T>(
    IAsyncEnumerable<T> Items,
    int TotalCount
);

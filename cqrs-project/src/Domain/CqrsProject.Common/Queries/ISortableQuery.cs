namespace CqrsProject.Common.Queries;

public interface ISortableQuery
{
    public string? SortBy { get; init; }
}

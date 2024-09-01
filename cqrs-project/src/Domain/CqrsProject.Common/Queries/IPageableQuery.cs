namespace CqrsProject.Common.Queries;

public interface IPageableQuery
{
    public int? Take { get; set; }
    public int? Skip { get; set; }
}

namespace CqrsProject.Core.Examples.Entities;

public static class ExampleConstrains
{
    public const short NameMaxLength = 200;
}

public class Example
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

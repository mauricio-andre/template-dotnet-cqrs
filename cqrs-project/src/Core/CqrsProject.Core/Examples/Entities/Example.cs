using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Examples.Entities;

public static class ExampleConstrains
{
    public const short NameMaxLength = 200;
}

public class Example
{
    public int Id { get; set; }

    [Required]
    [Unicode(false)]
    [MaxLength(ExampleConstrains.NameMaxLength)]
    public string Name { get; set; } = string.Empty;
}

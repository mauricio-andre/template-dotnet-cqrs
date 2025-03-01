using Bogus;

namespace CqrsProject.App.RestServerTest.V1.Me;

public class MeSyncTestFake
{
    public MeSyncTestFake()
    {
        Faker = new Faker<MeSyncTestFake>()
            .RuleFor(me => me.Email, fake => fake.Person.Email)
            .RuleFor(me => me.Username, fake => fake.Person.FirstName)
            .RuleFor(me => me.NameIdentifier, fake => string.Concat(fake.Person.Email, "|", fake.UniqueIndex));
    }

    public Faker<MeSyncTestFake> Faker { get; init; }

    public MeSyncTestFake Generate() => Faker.Generate();

    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string NameIdentifier { get; set; } = string.Empty;
}

namespace CqrsProject.CustomResourceWriter;

public static class Program
{
    public static void Main(string[] args)
    {
        var indexOutput = Array.FindIndex(args, x => x == "--output");
        var destinationPath = indexOutput >= 0
            ? args[indexOutput + 1]
            : Path.Combine(Environment.CurrentDirectory, "Localization", "Resources");

        JsonToResourcesConverter.Handler(destinationPath);
    }
}

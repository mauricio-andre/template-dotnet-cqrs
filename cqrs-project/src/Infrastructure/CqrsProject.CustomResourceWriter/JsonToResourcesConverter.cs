using System.Resources;
using System.Text.Json;
using CqrsProject.Common.Localization;

namespace CqrsProject.CustomResourceWriter;

public static class JsonToResourcesConverter
{
    public static void Handler(string destinationPath)
    {
        var jsonDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Jsons");
        var jsonFiles = Directory.GetFiles(jsonDirectory, "*.json");

        var classType = typeof(CqrsProjectResource);
        var projectName = classType.Module.Assembly.GetName().Name;
        var className = classType.FullName?.Replace($"{projectName}.", "");

        foreach (var jsonFilePath in jsonFiles)
        {
            var culture = Path.GetFileNameWithoutExtension(jsonFilePath);
            var resourcesFilePath = Path.Combine(destinationPath, $"{className}.{culture}.resources");

            ConvertJsonToResources(jsonFilePath, resourcesFilePath);
        }
    }

    private static void ConvertJsonToResources(string jsonFilePath, string resourcesFilePath)
    {
        using (var stream = new FileStream(jsonFilePath, FileMode.Open, FileAccess.Read))
        {
            var jsonDocument = JsonDocument.Parse(stream);
            using (var resourceWriter = new ResourceWriter(resourcesFilePath))
            {
                foreach (var element in jsonDocument.RootElement.EnumerateObject())
                {
                    resourceWriter.AddResource(element.Name, element.Value.GetString());
                }

                resourceWriter.Close();
            }
        }
    }
}

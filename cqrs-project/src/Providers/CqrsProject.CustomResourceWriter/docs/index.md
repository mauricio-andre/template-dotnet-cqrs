# Configuração

Para configurar esse recurso é necessário colocar o seguinte conteúdo no arquivo *.csproj*

```csproj
  <Target Name="RunCustomResourceWriter" BeforeTargets="BeforeBuild">
    <Exec Command="dotnet run --project ../../Providers/CqrsProject.CustomResourceWriter/CqrsProject.CustomResourceWriter.csproj --output ../../Core/CqrsProject.Common/Localization/Resources" />
    <Exec Command="dotnet build ../../Core/CqrsProject.Common/CqrsProject.Common.csproj" />
  </Target>
  <ItemGroup>
    <EmbeddedResource Include="Resources\**\*.resources" />
  </ItemGroup>
```

As configurações do comando *Target* fazem com que o comando seja executado antes do build completo do projeto, o primeiro comando executa o projeto *CustomResourceWriter* que vai gerar os arquivos *.resources* no caminho de saída informado, o segundo comando é necessário para fazer com que o projeto *Common* seja recarregado agora com os arquivos *.resources* disponíveis, como *CustomResourceWriter* faz uso do *Common* esse build é necessário.
O comando *EmbeddedResource* inclui os arquivos de resources gerados na pasta de saída.

No arquivo *program.cs* acrescente a seguinte linha:

```c#
builder.Services.AddLocalization(options => options.ResourcesPath = Path.Combine("Localization", "Resources"));
```

Essa configuração define o caminho base dos arquivos de *.resources* do projeto, que precisa estar alinhado com as configurações feitas no arquivo *.csproj*

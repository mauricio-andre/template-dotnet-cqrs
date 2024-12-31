# Configuração

Para configurar esse projeto basta colocar a seguinte linha no arquivo *program.cs*

```c#
builder.Services.AddCustomStringLocalizerProvider();
```

Isso fará com que todas as injeções de `IStringLocalizer<CqrsProjectResource>` carreguem o conteúdo do arquivo json do projeto.

Caso o projeto possua dependências de bibliotecas que precisam acessar o StringLocalizer com seu conteúdo escrito em arquivos *.resources*, basta incluir a seguinte linha para que a injeção padrão *IStringLocalizer* de outras classes se tornem acessíveis

```c#
builder.Services.AddLocalization();
```

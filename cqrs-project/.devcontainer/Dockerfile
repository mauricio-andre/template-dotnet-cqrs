FROM mcr.microsoft.com/devcontainers/dotnet:10.0

RUN apt-get update \
  && apt-get install -y \
    curl \
    git \
    vim \
  # Instalação da última versão LTS do Node.js
  && curl -fsSL https://deb.nodesource.com/setup_lts.x | bash - \
  && apt-get install -y nodejs \
  && rm -rf /var/lib/apt/lists/* \
  # Instalação de ferramentas .NET
  && dotnet tool install --global dotnet-ef

USER vscode

ENV SSL_CERT_DIR="${SSL_CERT_DIR:+$SSL_CERT_DIR:}/home/vscode/.aspnet/dev-certs/trust"

RUN dotnet dev-certs https --trust

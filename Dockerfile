FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine

# Diretório padrão que será usado no container
WORKDIR /app

# Instalando utilitários
RUN apk add --no-cache nano bash netcat-openbsd dos2unix

# Instalando EF CLI
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

# Copiando os .csproj mantendo a estrutura
COPY src/RO.DevTest.Application/RO.DevTest.Application.csproj ./src/RO.DevTest.Application/
COPY src/RO.DevTest.Domain/RO.DevTest.Domain.csproj ./src/RO.DevTest.Domain/
COPY src/RO.DevTest.Infrastructure/RO.DevTest.Infrastructure.csproj ./src/RO.DevTest.Infrastructure/
COPY src/RO.DevTest.Persistence/RO.DevTest.Persistence.csproj ./src/RO.DevTest.Persistence/
COPY src/RO.DevTest.Tests/RO.DevTest.Tests.csproj ./src/RO.DevTest.Tests/
COPY src/RO.DevTest.WebApi/RO.DevTest.WebApi.csproj ./src/RO.DevTest.WebApi/

# Restaurando dependências
RUN dotnet restore ./src/RO.DevTest.WebApi/RO.DevTest.WebApi.csproj

# Copiando o restante dos arquivos
COPY . .

# Corrigindo terminações de linha do script
RUN dos2unix /app/Scripts/Bash/entrypoint.sh

# Dando permissão de execução no script
RUN chmod +x /app/Scripts/Bash/entrypoint.sh

# Buildando o projeto
WORKDIR /app/src/RO.DevTest.WebApi
RUN dotnet clean
RUN dotnet build

# Expondo a porta
EXPOSE 5087

# Voltando pro diretório base
WORKDIR /app

# Rodando o entrypoint
ENTRYPOINT ["/bin/bash", "/app/Scripts/Bash/entrypoint.sh"]
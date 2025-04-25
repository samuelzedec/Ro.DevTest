FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine

# Diretório padrão que seá usado no container
WORKDIR app
RUN apk add --no-cache nano
RUN apk add --no-cache bash
RUN apk add --no-cache netcat-openbsd

# Baixando o EF CLI 
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

# Copiando todos os .csproj mantendo a estrutura
COPY src/RO.DevTest.Application/RO.DevTest.Application.csproj ./src/RO.DevTest.Application/
COPY src/RO.DevTest.Domain/RO.DevTest.Domain.csproj ./src/RO.DevTest.Domain/
COPY src/RO.DevTest.Infrastructure/RO.DevTest.Infrastructure.csproj ./src/RO.DevTest.Infrastructure/
COPY src/RO.DevTest.Persistence/RO.DevTest.Persistence.csproj ./src/RO.DevTest.Persistence/
COPY src/RO.DevTest.Tests/RO.DevTest.Tests.csproj ./src/RO.DevTest.Tests/
COPY src/RO.DevTest.WebApi/RO.DevTest.WebApi.csproj ./src/RO.DevTest.WebApi/

# Restaura dependências
RUN ["dotnet", "restore", "./src/RO.DevTest.WebApi/RO.DevTest.WebApi.csproj"]

# Copiando o restante dos arquivos
COPY . .

# Preparando o script de inicialização
RUN chmod +x /app/Scripts/Bash/entrypoint.sh

# Entrando na WebApi e executando clean/build
WORKDIR /app/src/RO.DevTest.WebApi
RUN ["dotnet", "clean"]
RUN ["dotnet", "build"]

# Expondo a porta
EXPOSE 5087

# Executando o script de inicialização
CMD ["/app/Scripts/Bash/entrypoint.sh"]
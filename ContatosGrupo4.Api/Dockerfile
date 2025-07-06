# build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copiar todos os csproj
COPY ["ContatosGrupo4.Api/ContatosGrupo4.Api.csproj", "ContatosGrupo4.Api/"]
COPY ["ContatosGrupo4.Application/ContatosGrupo4.Application.csproj", "ContatosGrupo4.Application/"]
COPY ["ContatosGrupo4.Infrastructure/ContatosGrupo4.Infrastructure.csproj", "ContatosGrupo4.Infrastructure/"]
COPY ["ContatosGrupo4.Domain/ContatosGrupo4.Domain.csproj", "ContatosGrupo4.Domain/"]

# restore
RUN dotnet restore "ContatosGrupo4.Api/ContatosGrupo4.Api.csproj"

# copiar tudo para build
COPY . .

WORKDIR "/src/ContatosGrupo4.Api"
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ContatosGrupo4.Api.dll"]
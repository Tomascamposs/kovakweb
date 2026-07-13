FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Modificamos esta línea para que busque adentro de la subcarpeta KovakWeb
COPY ["KovakWeb/KovakWeb.csproj", "KovakWeb/"]
RUN dotnet restore "KovakWeb/KovakWeb.csproj"
COPY . .
WORKDIR "/src/KovakWeb"
RUN dotnet build "KovakWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KovakWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KovakWeb.dll"]
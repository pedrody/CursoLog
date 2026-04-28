FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/CursoLog.WebApi/CursoLog.WebApi.csproj", "src/CursoLog.WebApi/"]
RUN dotnet restore "./src/CursoLog.WebApi/CursoLog.WebApi.csproj"
COPY . .
WORKDIR "/src/src/CursoLog.WebApi"
RUN dotnet build "CursoLog.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CursoLog.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CursoLog.WebApi.dll"]
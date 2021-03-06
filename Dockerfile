FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 5000

# get stuff and build
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /code
COPY . .
RUN dotnet restore itiValidaSenha.sln \
    && dotnet build itiValidaSenha.sln -c Release -o /app/build

# run unit tests
WORKDIR /code/test/ValidaSenha.Api.Test.UnitTest
RUN dotnet test ValidaSenha.Api.UnitTest.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# publish
FROM build AS publish
WORKDIR /code/src/ValidaSenha.Api
RUN dotnet publish ValidaSenha.Api.csproj -c Release -o /app/publish

# run
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ValidaSenha.Api.dll"]

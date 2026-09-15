FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY CC_Demo.sln ./
COPY CC_Demo/CC_Demo.csproj CC_Demo/
COPY CC_Demo.Cli/CC_Demo.Cli.csproj CC_Demo.Cli/
COPY CC_Demo.Tests/CC_Demo.Tests.csproj CC_Demo.Tests/
RUN dotnet restore CC_Demo/CC_Demo.csproj

COPY . .
RUN dotnet publish CC_Demo/CC_Demo.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "CC_Demo.dll"]

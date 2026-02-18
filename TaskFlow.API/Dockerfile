FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY TaskFlow.API.sln ./
COPY TaskFlow.API/TaskFlow.API.csproj TaskFlow.API/
RUN dotnet restore TaskFlow.API/TaskFlow.API.csproj

COPY TaskFlow.API/. TaskFlow.API/
RUN dotnet publish TaskFlow.API/TaskFlow.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "TaskFlow.API.dll"]

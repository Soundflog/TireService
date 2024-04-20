#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TireService/TireService.csproj", "TireService/"]
RUN dotnet restore "TireService/TireService.csproj"
COPY . .
WORKDIR "/src/TireService"
RUN dotnet build "TireService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TireService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TireService.dll"]
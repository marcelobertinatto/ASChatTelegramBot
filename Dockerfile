#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/aspnet:5.0-stretch-slim AS base
#FROM  mcr.microsoft.com/dotnet/aspnet:5.0 as base
FROM  mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

#FROM mcr.microsoft.com/dotnet/sdk:5.0-stretch-slim AS build
#FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY ["ChatTelegramBot.csproj", "ChatTelegramBot/"]
RUN dotnet restore "ChatTelegramBot/ChatTelegramBot.csproj"
WORKDIR "/src/ChatTelegramBot"
COPY . .
RUN dotnet build "ChatTelegramBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatTelegramBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet ChatTelegramBot.dll
#ENTRYPOINT ["dotnet", "ChatTelegramBot.dll"]
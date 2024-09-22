FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["services/Messages/Messages.SignalRNotifier/Messages.SignalRNotifier.csproj", "services/Messages/Messages.SignalRNotifier/"]
COPY ["services/Messages/Messages.Contracts/Messages.Contracts.csproj", "services/Messages/Messages.Contracts/"]
COPY ["services/Shared/Shared.Domain/Shared.Domain.csproj", "services/Shared/Shared.Domain/"]
COPY ["services/Shared/Shared.Abstractions/Shared.Abstractions.csproj", "services/Shared/Shared.Abstractions/"]
COPY ["services/Shared/Shared.Web/Shared.Web.csproj", "services/Shared/Shared.Web/"]
RUN dotnet restore "services/Messages/Messages.SignalRNotifier/Messages.SignalRNotifier.csproj"
COPY . .
WORKDIR "/src/services/Messages/Messages.SignalRNotifier"
RUN dotnet build "Messages.SignalRNotifier.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Messages.SignalRNotifier.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Messages.SignalRNotifier.dll"]

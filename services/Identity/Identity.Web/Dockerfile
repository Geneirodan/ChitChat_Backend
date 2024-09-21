FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["services/Identity/Identity.Web/Identity.Web.csproj", "services/Identity/Identity.Web/"]
COPY ["services/Identity/Identity.Persistence/Identity.Persistence.csproj", "services/Identity/Identity.Persistence/"]
COPY ["services/Shared/Shared.Domain/Shared.Domain.csproj", "services/Shared/Shared.Domain/"]
COPY ["services/Shared/Shared.Abstractions/Shared.Abstractions.csproj", "services/Shared/Shared.Abstractions/"]
COPY ["services/Shared/Shared.Web/Shared.Web.csproj", "services/Shared/Shared.Web/"]
COPY ["services/Identity/Identity.Emails/Identity.Emails.csproj", "services/Identity/Identity.Emails/"]
RUN dotnet restore "services/Identity/Identity.Web/Identity.Web.csproj"
COPY . .
WORKDIR "/src/services/Identity/Identity.Web"
RUN dotnet build "Identity.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Identity.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#COPY ["services/Identity/Identity.Emails/Views", "Views/"]
ENTRYPOINT ["dotnet", "Identity.Web.dll"]

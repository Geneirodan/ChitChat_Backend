FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["services/Messages/Queries/Messages.Queries.Web/Messages.Queries.Web.csproj", "services/Messages/Queries/Messages.Queries.Web/"]
COPY ["services/Messages/Queries/Messages.Queries.MassTransit/Messages.Queries.MassTransit.csproj", "services/Messages/Queries/Messages.Queries.MassTransit/"]
COPY ["services/Messages/Queries/Messages.Queries.Persistence/Messages.Queries.Persistence.csproj", "services/Messages/Queries/Messages.Queries.Persistence/"]
COPY ["services/Shared/Shared.EntityFramework/Shared.EntityFramework.csproj", "services/Shared/Shared.EntityFramework/"]
COPY ["services/Shared/Shared.Abstractions/Shared.Abstractions.csproj", "services/Shared/Shared.Abstractions/"]
COPY ["services/Messages/Messages.Contracts/Messages.Contracts.csproj", "services/Messages/Messages.Contracts/"]
COPY ["services/Shared/Shared.Domain/Shared.Domain.csproj", "services/Shared/Shared.Domain/"]
COPY ["services/Messages/Queries/Messages.Queries.Application/Messages.Queries.Application.csproj", "services/Messages/Queries/Messages.Queries.Application/"]
COPY ["services/Shared/Shared.MediatR/Shared.MediatR.csproj", "services/Shared/Shared.MediatR/"]
COPY ["services/Shared/Shared.Web/Shared.Web.csproj", "services/Shared/Shared.Web/"]
RUN dotnet restore "services/Messages/Queries/Messages.Queries.Web/Messages.Queries.Web.csproj"
COPY . .
WORKDIR "/src/services/Messages/Queries/Messages.Queries.Web"
RUN dotnet build "Messages.Queries.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Messages.Queries.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Messages.Queries.Web.dll"]

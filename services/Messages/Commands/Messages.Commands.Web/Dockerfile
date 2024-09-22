FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["services/Messages/Commands/Messages.Commands.Web/Messages.Commands.Web.csproj", "services/Messages/Commands/Messages.Commands.Web/"]
COPY ["services/Shared/Shared.Web/Shared.Web.csproj", "services/Shared/Shared.Web/"]
COPY ["services/Shared/Shared.Domain/Shared.Domain.csproj", "services/Shared/Shared.Domain/"]
COPY ["services/Shared/Shared.Abstractions/Shared.Abstractions.csproj", "services/Shared/Shared.Abstractions/"]
COPY ["services/Messages/Commands/Messages.Commands.MassTransit/Messages.Commands.MassTransit.csproj", "services/Messages/Commands/Messages.Commands.MassTransit/"]
COPY ["services/Messages/Messages.Contracts/Messages.Contracts.csproj", "services/Messages/Messages.Contracts/"]
COPY ["services/Messages/Commands/Messages.Commands.Persistence/Messages.Commands.Persistence.csproj", "services/Messages/Commands/Messages.Commands.Persistence/"]
COPY ["services/Shared/Shared.Marten/Shared.Marten.csproj", "services/Shared/Shared.Marten/"]
COPY ["services/Messages/Commands/Messages.Commands.Application/Messages.Commands.Application.csproj", "services/Messages/Commands/Messages.Commands.Application/"]
COPY ["services/Shared/Shared.MediatR/Shared.MediatR.csproj", "services/Shared/Shared.MediatR/"]
COPY ["services/Messages/Commands/Messages.Commands.Domain/Messages.Commands.Domain.csproj", "services/Messages/Commands/Messages.Commands.Domain/"]
RUN dotnet restore "services/Messages/Commands/Messages.Commands.Web/Messages.Commands.Web.csproj"
COPY . .
WORKDIR "/src/services/Messages/Commands/Messages.Commands.Web"
RUN dotnet build "Messages.Commands.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Messages.Commands.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Messages.Commands.Web.dll"]

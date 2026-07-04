FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/BasketManagement.API/BasketManagement.API.csproj", "BasketManagement.API/"]
COPY ["src/BasketManagement.Application/BasketManagement.Application.csproj", "BasketManagement.Application/"]
COPY ["src/BasketManagement.Domain/BasketManagement.Domain.csproj", "BasketManagement.Domain/"]
COPY ["src/BasketManagement.Infrastructure/BasketManagement.Infrastructure.csproj", "BasketManagement.Infrastructure/"]
RUN dotnet restore "BasketManagement.API/BasketManagement.API.csproj"
COPY . .
WORKDIR "/src/BasketManagement.API"
RUN dotnet build "BasketManagement.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BasketManagement.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BasketManagement.API.dll"]
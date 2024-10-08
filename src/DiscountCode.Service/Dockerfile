FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
#COPY . . 
COPY ["DiscountCode.Service/DiscountCode.Service.csproj", "DiscountCode.Service/"]
COPY ["DiscountCode.Domain/DiscountCode.Domain.csproj", "DiscountCode.Domain/"]
COPY ["DiscountCode.Application/DiscountCode.Application.csproj", "DiscountCode.Application/"]
COPY ["DiscountCode.Infrastructure/DiscountCode.Infrastructure.csproj", "DiscountCode.Infrastructure/"]

RUN dotnet restore "DiscountCode.Service/DiscountCode.Service.csproj"
COPY . .
WORKDIR "/src/DiscountCode.Service"
RUN dotnet build "DiscountCode.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DiscountCode.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "DiscountCode.Service.dll"]
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

COPY --from=build /src/. /src/.
ENTRYPOINT dotnet ef database update --project /src/DiscountCode.Infrastructure/DiscountCode.Infrastructure.csproj && dotnet DiscountCode.Service.dll

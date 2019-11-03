FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /service

# Copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore

# copy everything else and build app
COPY . .
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
WORKDIR /service
COPY --from=build /service/out ./
ENTRYPOINT ["dotnet", "Service.dll"]
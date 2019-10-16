FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /service

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime
WORKDIR /service
COPY --from=build-env /service/out .
ENTRYPOINT ["dotnet", "Department.dll"]
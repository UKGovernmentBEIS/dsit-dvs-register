FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY DVSRegister.sln ./
COPY DVSRegister/*.csproj DVSRegister/
COPY DVSRegister.BusinessLogic/*.csproj DVSRegister.BusinessLogic/
COPY DVSRegister.CommonUtility/*.csproj DVSRegister.CommonUtility/
COPY DVSRegister.Data/*.csproj DVSRegister.Data/

RUN dotnet restore DVSRegister/DVSRegister.csproj

COPY . .

RUN dotnet publish DVSRegister/DVSRegister.csproj \
    -c Release \
    -o /app/out \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/out ./

USER app

ENTRYPOINT ["dotnet", "DVSRegister.dll"]

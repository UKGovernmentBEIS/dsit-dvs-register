FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY *.sln .
COPY DVSRegister/*.csproj DVSRegister/
COPY DVSRegister.BusinessLogic/*.csproj DVSRegister.BusinessLogic/
COPY DVSRegister.CommonUtility/*.csproj DVSRegister.CommonUtility/
COPY DVSRegister.Data/*.csproj DVSRegister.Data/

RUN dotnet restore DVSRegister/

COPY . .
RUN dotnet publish -c Release -o out

# final image stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/out . 
ENTRYPOINT ["dotnet", "DVSRegister.dll"]

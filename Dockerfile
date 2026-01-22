FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore "ExpenseTrackerAPI.csproj"

COPY . ./
RUN dotnet publish "ExpenseTrackerAPI.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /App
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "ExpenseTrackerAPI.dll"]
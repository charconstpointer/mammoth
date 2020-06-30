FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-focal AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-focal AS build
WORKDIR /src
COPY [".", "app/"]
RUN dotnet restore "app/Mammoth.Worker/Mammoth.Worker.csproj"
COPY . .
WORKDIR "/src/Mammoth.Worker"
RUN dotnet build "Mammoth.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mammoth.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mammoth.Worker.dll"]
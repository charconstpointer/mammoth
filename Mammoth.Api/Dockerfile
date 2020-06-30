FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-focal AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5010
EXPOSE 5010

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-focal AS build
WORKDIR /src
COPY [".", "app/"]
RUN dotnet restore "app/Mammoth.Api/Mammoth.Api.csproj"
COPY . .
WORKDIR "/src/Mammoth.Api"
RUN dotnet build "Mammoth.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mammoth.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mammoth.Api.dll"]
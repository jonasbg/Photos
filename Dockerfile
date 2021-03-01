#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:latest AS build
WORKDIR /src
COPY ["photos.csproj", "."]
RUN dotnet restore "photos.csproj"
COPY . .
RUN dotnet build "photos.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "photos.csproj" \
    --runtime alpine-arm64 \
   # --self-contained true \
    /p:PublishTrimmed=true \
    /p:PublishSingleFile=true \
    -c Release \
    -o /app/publish

FROM base AS final

RUN adduser \
  --disabled-password \
  --home /app \
  --gecos '' app \
  && chown -R app /app
USER app

WORKDIR /app
COPY --from=publish /app/publish .
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 \
  DOTNET_RUNNING_IN_CONTAINER=true \
  ASPNETCORE_URLS=http://+:8080

EXPOSE 8080
#ENTRYPOINT ["dotnet", "photos.dll"]
ENTRYPOINT [ "./photos" ]
#ENTRYPOINT ["./photos", "--urls", "http://0.0.0.0:8080"]

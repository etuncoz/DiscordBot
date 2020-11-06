#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

#FROM mcr.microsoft.com/dotnet/core/runtime:3.1-nanoserver-1903 AS base
#WORKDIR /app
#
#FROM mcr.microsoft.com/dotnet/core/sdk:3.1-nanoserver-1903 AS build
#WORKDIR /src
#COPY ["DiscordBot/DiscordBot.csproj", "DiscordBot/"]
#RUN dotnet restore "DiscordBot/DiscordBot.csproj"
#COPY . .
#WORKDIR "/src/DiscordBot"
#RUN dotnet build "DiscordBot.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "DiscordBot.csproj" -c Release -o /app/publish
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "DiscordBot.dll"]

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .

# Run the app on container startup
# Use your project name for the second parameter
# e.g. MyProject.dll
#ENTRYPOINT [ "dotnet", "DiscordBot.dll" ]

CMD ASPNETCORE_URLS=http://*:$PORT dotnet DiscordBot.dll
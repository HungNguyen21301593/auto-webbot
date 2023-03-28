FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
EXPOSE 80

COPY ["./bin/Debug/net6.0","/app"]
ENTRYPOINT ["dotnet", "WebApi.dll"]
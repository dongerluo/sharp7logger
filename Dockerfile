FROM microsoft/dotnet:2.1-sdk
WORKDIR /app

# copy csproj and restore as a distinct layer
COPY *.csproj ./
RUN dotnet restore

# copy and build everything else
COPY . ./
RUN dotnet publish -c Release -o out
ENTRYPOINT ["dotnet", "out/sharp7logger.dll"]
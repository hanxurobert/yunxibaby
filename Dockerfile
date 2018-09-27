FROM microsoft/dotnet:2.1-sdk AS build-env
WORKDIR /app
ENV NODE_VERSION 10.11.0
ENV NODE_DOWNLOAD_SHA 4d8aaf8c1c51acbbb46bbd4e3c924a573884603b1c4e35cc02982bbda9779c8b
RUN curl -SL "https://nodejs.org/dist/v${NODE_VERSION}/node-v${NODE_VERSION}-linux-x64.tar.gz" --output nodejs.tar.gz \
    && echo "$NODE_DOWNLOAD_SHA nodejs.tar.gz" | sha256sum -c - \
    && tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
    && rm nodejs.tar.gz \
    && ln -s /usr/local/bin/node /usr/local/bin/nodejs

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out -r linux-x64

# Build runtime image
FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build-env /app/out .
# Make port 80 available to the world outside this container
EXPOSE 80
ENTRYPOINT [ "dotnet", "YunxiBaby.dll" ]
FROM mcr.microsoft.com/dotnet/sdk:3.1 as builder
WORKDIR /work
COPY . .
RUN dotnet build -c Release src/Redis.Stream.Subscriber.Publisher/Redis.Stream.Subscriber.Publisher.csproj -o out
FROM mcr.microsoft.com/dotnet/runtime:3.1
WORKDIR /app
COPY --from=builder /work/out .
CMD dotnet Redis.Stream.Subscriber.Publisher.dll
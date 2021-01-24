
# Redis.Stream.Subscriber
## Description
The StackExchange.Redis client does not provide the functionality to subscribe a stream without constant polling. 

https://github.com/StackExchange/StackExchange.Redis/issues/1155

The purpose of this library is to provide a more "EventStore" style blocking stream client for redis.
Where possible/feasible we will attempt to keep this redis client as similar as possible to the latest EventStore client implementation
## Table of Contents
* [Installation](#installation)
* [Methodology](#methodology)
* [Usage](#usage)
* [License](#license)

## Installation
Install via nuget
```
dotnet add PROJECT package Redis.Stream.Subscriber
```

## Methodology

The primary purpose of this library is to avoid  polling redis for stream updates. 

To achieve this we instead use a direct socket connection to the redis server using the dotnet TcpClient, we write the Stream commands directly to redis and then listen for data to be transmitted back on the same socket.
The thread is blocked listening on the socket untill there is data consume, at which point it is read and serialized to a "StreamEntry" object and made available to you to process however you see fit 

## Usage

Initialize tcp connection with redis
```c#
var connect = connection.Connect(new RedisStreamSettings
{
    host = "localhost",
    Port = 6379
});
```

Start receiving stream entries
```c#
uint startingIndex = 0;
var entries = connect.ReadStreamAsync("mystream", startingIndex);
await foreach (var entry in entries)
{
    await Console.Out.WriteLineAsync(entry.Data);
}
```

### Examples

### Redis
this repo contains a docker-compose file with a sample redis setup you can use to run the example clients in this solution

`docker-compose up`

you can then connect to redis-commander to view your stream http://localhost:8081/

### Listener and Publisher

#### Listener
```c#
dotnet run --project src/Redis.Stream.Subscriber.Listener/Redis.Stream.Subscriber.Listener.csproj
```

#### Publisher
this application used the StackExchange.Redis library to publish changes
```c#
dotnet run --project ./src/Redis.Stream.Subscriber.Publisher/Redis.Stream.Subscriber.Publisher.csproj
```

## License
TBD...
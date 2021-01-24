
# Redis.Stream.Subscriber
## Description
The StackExchange.Redis client does not provide the functionality to subscribe a stream without constant polling. 

https://github.com/StackExchange/StackExchange.Redis/issues/1155

The purpose of this library is to provide a more "EventStore" blocking style redis stream client. 

## Table of Contents
* [Installation](#installation)
* [Usage](#usage)
* [Methodology](#methodology)
* [License](#license)

## Installation
Install via nuget
```
dotnet add PROJECT package Redis.Stream.Subscriber
```

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
### Redis

this repo contains a docker-compose file with a sample redis setup you can use to run the example clients in this solution

`docker-compose up`

you can then connect to redis-commander on port


## License
TBD...
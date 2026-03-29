# Redis.Stream.Subscriber
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![Build Status][build-shield]][build-status]
[![MIT License][license-shield]][license-url]

## Description
The StackExchange.Redis client does not provide the functionality to subscribe a stream without constant polling. 

https://github.com/StackExchange/StackExchange.Redis/issues/1155

The purpose of this library is to provide a more "EventStore" style blocking stream client for redis and where possible/feasible we will attempt to keep this redis client as similar as possible to the latest EventStore client implementation

This project is concerned only with reading streams. It provides no functionality to write to redis at all. If you require this functionality we recommend using the **StackExchange.Redis** package

## Table of Contents
* [Features](#features)
* [Installation](#installation)
* [Methodology](#methodology)
* [Usage](#usage)
* [Building](#building)
* [License](#license)


## Features
- [X] Read stream forward (XREAD)
- [X] Read stream backwards (XREVRANGE)
- [X] Configurable subscription settings
- [X] Input validation

### Subscription Settings
- `BufferSize` - Socket buffer size for reading data (default: 1024 bytes)
- `BatchSize` - Number of stream entries to read per batch (default: 1)
- `Validate()` - Validates settings before use

### Redis Stream Settings
- `Host` - Redis server hostname or IP address (required)
- `Port` - Redis server port (default: 6379)
- `Timeout` - Connection timeout in milliseconds (default: 100)
- `Validate()` - Validates host, port, and timeout values

## Installation
Install via nuget
```bash
dotnet add PROJECT package Redis.Stream.Subscriber
```

## Methodology

The primary purpose of this library is to avoid  polling redis for stream updates. 

To achieve this we instead use a direct socket connection to the redis server using the dotnet TcpClient, we write the Stream commands directly to redis and then listen for data to be transmitted back on the same socket.
The thread is blocked listening on the socket until there is data consume, at which point it is read and serialized to a "StreamEntry" object and made available to you to process however you see fit. 

## Usage

Initialize connection with redis using RedisStreamSettings
```c#
var connection = new RedisConnection();
var connect = connection.Connect(new RedisStreamSettings
{
    Host = "localhost",
    Port = 6379,
    Timeout = 100
});
```

Configure subscription settings and start receiving stream entries
```c#
var settings = new SubscriptionSettings
{
    BufferSize = 1024,
    BatchSize = 1
};

uint startingIndex = 0;
var entries = connect.ReadStreamAsync("mystream", startingIndex, settings);
await foreach (var entry in entries)
{
    await Console.Out.WriteLineAsync(entry.Data);
}
```

Read stream backwards
```c#
var entries = connect.ReadStreamBackwardsAsync("mystream", "$", 10);
await foreach (var entry in entries)
{
    await Console.Out.WriteLineAsync(entry.Data);
}
```

Close the connection
```c#
connect.Close();
```

## Building
### Windows / Linux / macOS
**Prerequisites**
- [.NET Core SDK 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)

or
- [Docker](https://docs.docker.com/desktop/)

### The very easy way
this repo contains a docker-compose file that will build and run the sample applications along with a redis instance you can use to run the sample clients in this solution

```bash
docker-compose up 
```

you can then connect to redis-commander to view your stream http://localhost:8081/

### The (slightly less) easy way
you will need a redis instance first before this application will run. you can set up your own
or use the one from the docker-compose file in this repository
```bash
docker-compose up redis
```

Build the project, there are no external dependencies so this should be fast
```c#
dotnet build Redis.Stream.Subscriber.sln
```

the publisher application uses the StackExchange.Redis library to publish changes which the listener project below will listen
```c#
dotnet run --project ./src/Redis.Stream.Subscriber.Publisher/Redis.Stream.Subscriber.Publisher.csproj
```

Run the stream listener project that will subscribe to a stream
```c#
dotnet run --project src/Redis.Stream.Subscriber.Listener/Redis.Stream.Subscriber.Listener.csproj
```

## License

Distributed under the MIT License. See `LICENSE` for more information.

[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![Build Status][build-shield]][build-status]
[![MIT License][license-shield]][license-url]

<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/jimfim/Redis.Stream.Subscriber.svg?style=for-the-badge
[contributors-url]: https://github.com/jimfim/Redis.Stream.Subscriber/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/jimfim/Redis.Stream.Subscriber.svg?style=for-the-badge
[forks-url]: https://github.com/jimfim/Redis.Stream.Subscriber/network/members
[stars-shield]: https://img.shields.io/github/stars/jimfim/Redis.Stream.Subscriber.svg?style=for-the-badge
[stars-url]: https://github.com/jimfim/Redis.Stream.Subscriber/stargazers
[issues-shield]: https://img.shields.io/github/issues/jimfim/Redis.Stream.Subscriber.svg?style=for-the-badge
[issues-url]: https://github.com/jimfim/Redis.Stream.Subscriber/issues
[license-shield]: https://img.shields.io/github/license/jimfim/Redis.Stream.Subscriber.svg?style=for-the-badge
[license-url]: https://github.com/jimfim/Redis.Stream.Subscriber/blob/master/LICENSE.txt
[build-shield]: https://img.shields.io/github/actions/workflow/status/jimfim/Redis.Stream.Subscriber/dotnet.yml?branch=main&style=for-the-badge
[build-status]: https://github.com/jimfim/Redis.Stream.Subscriber/actions
